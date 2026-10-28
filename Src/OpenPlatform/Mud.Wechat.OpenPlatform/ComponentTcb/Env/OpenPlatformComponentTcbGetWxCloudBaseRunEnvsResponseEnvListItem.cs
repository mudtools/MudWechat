// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>获取云托管环境信息·env_list(Array)。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_getwxcloudbaserunenvs.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItem
{
    /// <summary>账户下该环境唯一标识</summary>
    [JsonPropertyName("env_id")]
    public string? EnvId { get; set; }

    /// <summary>环境来源。包含以下取值：miniapp：微信小程序qcloud ：腾讯云</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>环境别名，要以 a-z 开头，不能包含 a-zA-z0-9- 以外的字符</summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>创建时间</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>最后修改时间</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>环境状态。包含以下取值：NORMAL：正常可用UNAVAILABLE：服务不可用，可能是尚未初始化或者初始化过程中</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>数据库列表</summary>
    [JsonPropertyName("databases")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemDatabases? Databases { get; set; }

    /// <summary>存储列表</summary>
    [JsonPropertyName("storages")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemStorages? Storages { get; set; }

    /// <summary>函数列表</summary>
    [JsonPropertyName("functions")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemFunctions? Functions { get; set; }

    /// <summary>tcb产品套餐ID</summary>
    [JsonPropertyName("package_id")]
    public string? PackageId { get; set; }

    /// <summary>套餐中文名称</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>云日志服务列表</summary>
    [JsonPropertyName("log_services")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemLogServices? LogServices { get; set; }

    /// <summary>静态资源信息</summary>
    [JsonPropertyName("static_storages")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemStaticStorages? StaticStorages { get; set; }

    /// <summary>是否到期自动降为免费版</summary>
    [JsonPropertyName("is_auto_degrade")]
    public bool? IsAutoDegrade { get; set; }

    /// <summary>环境渠道</summary>
    [JsonPropertyName("env_channel")]
    public string? EnvChannel { get; set; }

    /// <summary>支付方式。包含以下取值： prepayment：预付费 postpaid：后付费</summary>
    [JsonPropertyName("pay_mode")]
    public string? PayMode { get; set; }

    /// <summary>是否为默认环境</summary>
    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; set; }

    /// <summary>环境所属地域</summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>环境标签列表</summary>
    [JsonPropertyName("tags")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemTags? Tags { get; set; }

    /// <summary>自定义日志服务</summary>
    [JsonPropertyName("custom_log_services")]
    public OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemCustomLogServices? CustomLogServices { get; set; }
}
