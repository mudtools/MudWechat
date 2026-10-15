// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>获取云开发环境列表·info_list(Array)。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_gettcbenvlist.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbGetTcbEnvListResponseInfoListItem
{
    /// <summary>环境ID</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>环境别名</summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>创建时间</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>最后修改时间</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>环境状态</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>tcb产品套餐ID</summary>
    [JsonPropertyName("package_id")]
    public string? PackageId { get; set; }

    /// <summary>套餐中文名称</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>数据库示例ID</summary>
    [JsonPropertyName("dbinstance_id")]
    public string? DbinstanceId { get; set; }

    /// <summary>静态存储ID</summary>
    [JsonPropertyName("bucket_id")]
    public string? BucketId { get; set; }
}
