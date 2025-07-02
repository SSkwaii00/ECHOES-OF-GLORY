using UnityEngine;
using UnityEditor;
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEditor.SceneManagement;

public class MeshyBridgeWindow : EditorWindow
{
    private static MeshyBridge bridgeInstance;
    private static bool isBridgeRunning = false;
    private GUIContent runButtonContent;
    private GUIContent stopButtonContent;

    [MenuItem("Meshy/Bridge")]
    public static void ShowWindow()
    {
        var window = GetWindow<MeshyBridgeWindow>("Meshy Bridge");
        window.minSize = new Vector2(250, 100);
        window.maxSize = new Vector2(400, 150);
    }

    private void OnEnable()
    {
        // 初始化按钮内容
        runButtonContent = new GUIContent("Run Bridge");
        stopButtonContent = new GUIContent("Bridge ON");

        // 检查桥接实例是否存在
        isBridgeRunning = bridgeInstance != null;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);

        // 创建一个自定义样式
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fixedHeight = 40;

        // 根据桥接状态设置按钮颜色
        Color originalColor = GUI.backgroundColor;
        if (isBridgeRunning)
        {
            GUI.backgroundColor = new Color(0.4f, 0.6f, 1.0f); // 蓝色表示运行中
        }

        // 显示状态切换按钮
        GUIContent currentContent = isBridgeRunning ? stopButtonContent : runButtonContent;
        if (GUILayout.Button(currentContent, buttonStyle))
        {
            ToggleBridgeState();
        }

        // 恢复原始颜色
        GUI.backgroundColor = originalColor;

        EditorGUILayout.EndVertical();
    }

    private void ToggleBridgeState()
    {
        if (isBridgeRunning)
        {
            StopBridge();
        }
        else
        {
            StartBridge();
        }
    }

    private static void StartBridge()
    {
        if (bridgeInstance == null)
        {
            var go = new GameObject("MeshyBridge");
            bridgeInstance = go.AddComponent<MeshyBridge>();
            isBridgeRunning = true;
            Debug.Log("Meshy Bridge started");
        }
    }

    private static void StopBridge()
    {
        if (bridgeInstance != null)
        {
            bridgeInstance.StopServer();
            DestroyImmediate(bridgeInstance.gameObject);
            bridgeInstance = null;
            isBridgeRunning = false;
            Debug.Log("Meshy Bridge stopped");
        }
    }

    // 确保窗口关闭时不会影响桥接状态
    private void OnDestroy()
    {
        // 窗口关闭时不停止桥接
    }
}

// 保留原有的菜单项，但将它们设为私有
public static class MeshyBridgeCommands
{
    private static void StartBridge()
    {
        MeshyBridgeWindow.ShowWindow();
        var windowType = typeof(MeshyBridgeWindow);
        var method = windowType.GetMethod("StartBridge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method.Invoke(null, null);
    }
}

[ExecuteInEditMode]
public class MeshyBridge : MonoBehaviour
{
    // 添加缓存路径字段
    private string _tempCachePath;

    private Thread serverThread;
    private Thread guardThread;
    private bool _serverStop = false;
    private TcpListener listener;
    private Queue<MeshTransfer> importQueue = new Queue<MeshTransfer>();

    [System.Serializable]
    public class MeshTransfer
    {
        public string file_format;
        public string path;
        public string name;
        public int frameRate; // 添加帧率字段
    }

    void Start()
    {
        Debug.Log("[Meshy Bridge] 开始初始化服务器");
        // 在主线程中预先获取临时路径
        _tempCachePath = Application.temporaryCachePath;
        try
        {
            StartServer();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Meshy Bridge] 启动失败: {e.Message}\n{e.StackTrace}");
        }
    }

    public void StartServer()
    {
        Debug.Log("[Meshy Bridge] 正在启动服务器线程");
        _serverStop = false;
        serverThread = new Thread(RunServer);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private void GuardJob()
    {
        while (!_serverStop)
        {
            Thread.Sleep(200);
        }

        if (listener != null)
        {
            listener.Stop();
            Debug.Log("[Meshy Bridge] Guard thread shutting down server");
        }
    }

    void RunServer()
    {
        listener = new TcpListener(IPAddress.Any, 5326);
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 添加套接字重用选项
        listener.Start();

        guardThread = new Thread(GuardJob);
        guardThread.IsBackground = true;
        guardThread.Start();

        Debug.Log("[Meshy Bridge] Listening on port 5326");

        while (!_serverStop)
        {
            if (listener.Pending())
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    ProcessClientRequest(stream);
                }
            }
            Thread.Sleep(100);
        }

        listener.Stop();
        Debug.Log("[Meshy Bridge] Server stopped");
    }

    public void StopServer()
    {
        _serverStop = true;

        if (serverThread != null && serverThread.IsAlive)
        {
            serverThread.Join();
        }

        if (guardThread != null && guardThread.IsAlive)
        {
            guardThread.Join();
        }
    }

    private readonly string[] allowedOrigins = new string[]
    {
        "https://www.meshy.ai",
        "https://app-staging.meshy.ai",
        "http://localhost:3700"
    };

    void ProcessClientRequest(NetworkStream stream)
    {
        try
        {
            Debug.Log("[Meshy Bridge] 开始处理客户端请求");
            byte[] buffer = new byte[1024 * 16]; // 增大缓冲区
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log($"[Meshy Bridge] 收到请求数据({bytesRead}字节):\n{request}");

            // 解析请求头 - 添加错误处理
            var requestLines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // 检查是否有足够的请求行
            if (requestLines.Length == 0)
            {
                Debug.LogWarning("[Meshy Bridge] 请求为空");
                SendErrorResponse(stream, "Empty request");
                return;
            }

            // 安全解析请求方法和路径
            string[] requestParts = requestLines[0].Split(' ');
            if (requestParts.Length < 2)
            {
                Debug.LogWarning("[Meshy Bridge] 请求行格式不正确: " + requestLines[0]);
                SendErrorResponse(stream, "Invalid request format");
                return;
            }

            string method = requestParts[0];
            string path = requestParts[1];
            string origin = GetHeaderValue(requestLines, "Origin");

            // 处理OPTIONS预检请求
            if (method == "OPTIONS")
            {
                SendOptionsResponse(stream, origin);
                return;
            }

            // 处理GET状态请求
            if (method == "GET" && (path == "/status" || path == "/ping"))
            {
                SendStatusResponse(stream, origin);
                return;
            }

            // 处理POST导入请求
            if (method == "POST" && path == "/import")
            {
                ProcessImportRequest(stream, request, origin);
                return;
            }

            // 未知路径
            SendNotFoundResponse(stream, origin);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Meshy Bridge] 请求处理异常: {e.GetType().Name} - {e.Message}\n{e.StackTrace}");
            try
            {
                SendErrorResponse(stream, e.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Meshy Bridge] 发送错误响应时出错: {ex.Message}");
            }
        }
    }

    // 添加响应数据结构
    [System.Serializable]
    private class ImportResponseData
    {
        public string status;
        public string message;
        public string path;
    }

    private void ProcessImportRequest(NetworkStream stream, string request, string origin)
    {



        try
        {
            // 提取JSON正文
            int jsonStart = request.IndexOf('{');
            if (jsonStart < 0)
            {
                Debug.LogWarning("[Meshy Bridge] 无法找到JSON开始标记");
                throw new Exception("Invalid request format: JSON not found");
            }

            string jsonBody = request.Substring(jsonStart);
            // 验证是否是有效JSON
            if (!jsonBody.Trim().StartsWith("{") || !jsonBody.Trim().EndsWith("}"))
            {
                Debug.LogError("无效的JSON格式");
                throw new Exception("Invalid JSON format");
            }
            Debug.Log($"[Meshy Bridge] 解析JSON: {jsonBody}");

            // 修改为使用自定义类解析JSON
            var data = JsonUtility.FromJson<ImportRequestData>(jsonBody);
            if (data == null)
            {
                Debug.LogWarning("[Meshy Bridge] JSON解析失败");
                throw new Exception("Failed to parse JSON data");
            }

            if (string.IsNullOrEmpty(data.url))
            {
                Debug.LogWarning("[Meshy Bridge] URL为空");
                throw new Exception("Missing required field: url");
            }

            if (string.IsNullOrEmpty(data.format))
            {
                Debug.LogWarning("[Meshy Bridge] 格式为空,使用默认值glb");
                data.format = "glb";
            }

            // 修正临时文件路径
            string fileName = $"bridge_model";
            string filePath = Path.Combine(Path.GetTempPath(), "Meshy", fileName);

            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            Debug.Log($"[Meshy Bridge] 开始下载文件: {data.url}");
            using (var client = new WebClient())
            {
                client.DownloadFile(data.url, filePath);
            }
            // 读取文件头判断文件类型
            string fileExtension = ".glb"; // 默认glb
            byte[] header = new byte[4];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header, 0, 4);
                if (header[0] == 'P' && header[1] == 'K' && header[2] == 0x03 && header[3] == 0x04)
                {
                    fileExtension = ".zip";
                }
                else if (header[0] == 'g' && header[1] == 'l' && header[2] == 'T' && header[3] == 'F')
                {
                    fileExtension = ".glb";
                }
            }

            // 重命名文件添加正确扩展名
            string finalPath = filePath + fileExtension;
            File.Move(filePath, finalPath);
            filePath = finalPath;
            Debug.Log($"[Meshy Bridge] 文件下载完成: {filePath}");

            // 加入导入队列
            lock (importQueue)
            {
                importQueue.Enqueue(new MeshTransfer
                {
                    file_format = data.format,
                    path = filePath,
                    name = data.name ?? "",
                    frameRate = data.frameRate
                });
            }

            // 发送成功响应
            var responseData = new ImportResponseData
            {
                status = "ok",
                message = "File queued for import",
                path = filePath
            };

            string jsonResponse = JsonUtility.ToJson(responseData);

            string response = string.Join("\r\n",
                $"HTTP/1.1 200 OK",
                $"Access-Control-Allow-Origin: {GetAllowedOrigin(origin)}",
                "Content-Type: application/json; charset=utf-8",
                "Connection: close",
                $"Content-Length: {jsonResponse.Length}",
                "",
                jsonResponse);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
            stream.Flush();
            Debug.Log("[Meshy Bridge] 导入请求处理成功");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Meshy Bridge] 处理导入请求失败: {e.Message}");
            SendErrorResponse(stream, e.Message);
        }
    }

    // 添加请求数据结构
    [System.Serializable]
    private class ImportRequestData
    {
        public string url;
        public string format;
        public string name;
        public int frameRate = 30; // 默认30帧
    }

    private string GetAllowedOrigin(string origin)
    {
        return Array.Exists(allowedOrigins, o => string.Equals(o, origin, StringComparison.OrdinalIgnoreCase)) ?
            origin : "https://www.meshy.ai";
    }

    private void SendOptionsResponse(NetworkStream stream, string origin)
    {
        string response = $"HTTP/1.1 200 OK\r\n" +
                        $"Access-Control-Allow-Origin: {GetAllowedOrigin(origin)}\r\n" +
                        "Access-Control-Allow-Methods: POST, GET, OPTIONS\r\n" +
                        "Access-Control-Allow-Headers: *\r\n" +
                        "Access-Control-Max-Age: 86400\r\n\r\n";
        stream.Write(Encoding.UTF8.GetBytes(response), 0, response.Length);
    }

    [System.Serializable]
    private class StatusResponseData
    {
        public string status = "ok";
        public string dcc = "unity";
        public string version;
    }

    private void SendStatusResponse(NetworkStream stream, string origin)
    {
        // 创建响应数据对象
        var responseData = new StatusResponseData
        {
            dcc = "unity",
            status = "ok",
            version = Application.unityVersion
        };

        // 转换为JSON
        string jsonResponse = JsonUtility.ToJson(responseData);

        // 构建完整HTTP响应
        string response = string.Join("\r\n",
            "HTTP/1.1 200 OK",
            $"Access-Control-Allow-Origin: {GetAllowedOrigin(origin)}",
            "Content-Type: application/json; charset=utf-8",
            "Connection: close",
            $"Content-Length: {jsonResponse.Length}",
            "",
            jsonResponse);

        // 发送响应
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        stream.Write(responseBytes, 0, responseBytes.Length);
        stream.Flush();

        Debug.Log($"[Meshy Bridge] 发送状态响应: {jsonResponse}");

    }

    private void SendNotFoundResponse(NetworkStream stream, string origin)
    {
        string response = $"HTTP/1.1 404 Not Found\r\n" +
                        $"Access-Control-Allow-Origin: {GetAllowedOrigin(origin)}\r\n" +
                        "Content-Type: application/json\r\n\r\n" +
                        JsonUtility.ToJson(new { status = "path not found" });
        stream.Write(Encoding.UTF8.GetBytes(response), 0, response.Length);
    }

    private void SendErrorResponse(NetworkStream stream, string message)
    {
        try
        {
            string jsonBody = JsonUtility.ToJson(new { status = "error", message });
            string response = string.Join("\r\n",
                "HTTP/1.1 500 Internal Server Error",
                "Access-Control-Allow-Origin: *",
                "Content-Type: application/json; charset=utf-8",
                "Connection: close",
                $"Content-Length: {jsonBody.Length}",
                "",
                jsonBody);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
            stream.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError($"发送错误响应失败: {e.Message}");
        }
    }

    private string GetHeaderValue(string[] headers, string headerName)
    {
        foreach (var header in headers)
        {
            if (header.StartsWith(headerName + ":"))
                return header.Substring(headerName.Length + 1).Trim();
        }
        return "";
    }

    void Update()
    {
        // 处理导入队列
        lock (importQueue)
        {
            while (importQueue.Count > 0)
            {
                var transfer = importQueue.Dequeue();
                ProcessMeshTransfer(transfer);
            }
        }
    }

    private void ProcessMeshTransfer(MeshTransfer transfer)
    {
        try
        {
            string fileExtension = Path.GetExtension(transfer.path)?.ToLower();
            switch (fileExtension)
            {
                case ".glb":
                    ImportModelWithMaterial(transfer);
                    break;
                case ".zip":
                    ProcessZipFile(transfer);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing mesh: {e.Message}");
        }
        finally
        {
            CleanupTempFile(transfer.path);
        }
    }

    private void ImportModelWithMaterial(MeshTransfer transfer)
    {
        try
        {
            // 确保目标目录存在
            string importDir = "Assets/MeshyImports";
            if (!Directory.Exists(importDir))
            {
                Directory.CreateDirectory(importDir);
                AssetDatabase.Refresh();
            }

            // 生成更有意义的文件名
            string modelName = string.IsNullOrEmpty(transfer.name) ? "Meshy_Model" : transfer.name;
            // 清理文件名中的非法字符
            modelName = string.Join("_", modelName.Split(Path.GetInvalidFileNameChars()));

            // 获取文件扩展名
            string extension = Path.GetExtension(transfer.path);
            if (string.IsNullOrEmpty(extension))
            {
                extension = $".{transfer.file_format}";
            }

            // 创建唯一文件名
            string uniqueFileName = $"{modelName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{extension}";
            string relativePath = Path.Combine(importDir, uniqueFileName);

            // 检查源文件是否存在
            if (!File.Exists(transfer.path))
            {
                Debug.LogError($"源文件不存在: {transfer.path}");
                return;
            }

            // 先复制文件到目标位置
            File.Copy(transfer.path, relativePath, true);

            AssetDatabase.ForceReserializeAssets(new[] { relativePath });

            // 导入模型
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);

            // 获取导入的模型
            GameObject importedObject = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
            if (importedObject != null)
            {
                // 使用传入的名称或生成的名称
                importedObject.name = uniqueFileName;

                // 添加默认材质
                AddDefaultMaterial(importedObject);

                // 确保修改保存
                EditorUtility.SetDirty(importedObject);
                AssetDatabase.SaveAssets();

                // 在主线程中实例化模型到场景
                EditorApplication.delayCall += () =>
                {
                    // 实例化模型到场景
                    GameObject sceneObject = PrefabUtility.InstantiatePrefab(importedObject) as GameObject;
                    if (sceneObject != null)
                    {
                        // 重置变换
                        sceneObject.transform.position = Vector3.zero;
                        sceneObject.transform.rotation = Quaternion.identity;
                        sceneObject.transform.localScale = Vector3.one;

                        // 选中新添加的对象
                        Selection.activeGameObject = sceneObject;

                        // 确保场景被标记为已修改
                        EditorSceneManager.MarkSceneDirty(sceneObject.scene);

                        Debug.Log($"[Meshy Bridge] 模型已添加到场景: {sceneObject.name}");
                    }
                };
            }
            Debug.Log($"[Meshy Bridge] 导入模型成功: {relativePath}, 名称: {uniqueFileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Meshy Bridge] 导入模型失败: {e.Message}\n{e.StackTrace}");
        }
    }

    private void AddDefaultMaterial(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterials.Length == 0)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Meshy_Material";
            renderer.sharedMaterial = material;
        }
    }

    private void ProcessZipFile(MeshTransfer transfer)
    {
        // 修改为使用预先获取的临时路径
        string extractPath = Path.Combine(_tempCachePath, "extracted");
        ZipFile.ExtractToDirectory(transfer.path, extractPath);

        // 处理解压后的文件
        foreach (string file in Directory.GetFiles(extractPath, "*.glb", SearchOption.AllDirectories))
        {
            MeshTransfer newTransfer = new MeshTransfer
            {
                file_format = "glb",
                path = file,
                name = transfer.name
            };
            ImportModelWithMaterial(newTransfer);
        }

        // 清理临时目录
        Directory.Delete(extractPath, true);
    }

    private void CleanupTempFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error cleaning up: {e.Message}");
        }
    }

    void OnDestroy()
    {
        StopServer();
    }
}