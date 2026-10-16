// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.SmartGuide;

/// <summary>服务人员查询响应·【服务人员列表】。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535123"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688558"/></summary>
[HttpJsonSerializable(SerializerClassName = "SmartGuide")]
public class SmartGuideListResponseDataItem
{
    /// <summary>【服务人员ID】 导购在导购系统中的唯一标识（官方字段路径 <c>data.guide_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("guide_id")]
    public string? GuideId { get; set; }

    /// <summary>【门店ID】 门店在微信支付商户平台的唯一标识（官方字段路径 <c>data.store_id</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("store_id")]
    public long? StoreId { get; set; }

    /// <summary>【服务人员姓名】员工在商户企业微信通讯录上的姓名，该字段需进行解密处理，具体解密方法详见 如何使用API证书解密敏感字段（官方字段路径 <c>data.name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【手机号码】服务人员通过小程序注册时填写的手机号码，使用微信商家不返回，该字段需进行解密处理，具体解密方法详见 如何使用API证书解密敏感字段（官方字段路径 <c>data.mobile</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>【企业微信的员工ID】 员工在商户企业微信通讯录使用的唯一标识，使用微信商家不返回（官方字段路径 <c>data.userid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>【工号】 服务人员通过小程序注册时填写的工号，使用微信商家不返回（官方字段路径 <c>data.work_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("work_id")]
    public string? WorkId { get; set; }

}

/// <summary>服务人员查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535123"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688558"/></summary>
[HttpJsonSerializable(SerializerClassName = "SmartGuide")]
public class SmartGuideListResponse : WechatPayResponse
{
    /// <summary>【服务人员列表】（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<SmartGuideListResponseDataItem>? Data { get; set; }

    /// <summary>【服务人员数量】 符合条件筛选的服务人员数量（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【最大资源条数】 该次请求可返回的最大资源条数，不大于10（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源的起始位置，默认值为0（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

}
