// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>数据库导出请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbexport.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbExportDatabaseRequest
{
    /// <summary>云环境ID</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>导出文件路径（文件会导出到公共的云存储中，可使用获取下载链接 API 获取下载链接）</summary>
    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    /// <summary>导出文件类型，1表示JSON；2表示CSV。文件格式参考数据库导入指引中的 文件格式 部分</summary>
    [JsonPropertyName("file_type")]
    public long? FileType { get; set; }

    /// <summary>导出条件.</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }
}
