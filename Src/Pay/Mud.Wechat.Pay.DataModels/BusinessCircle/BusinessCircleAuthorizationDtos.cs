// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BusinessCircle;

/// <summary>商圈会员积分服务授权查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534848"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474135"/></summary>
[HttpJsonSerializable(SerializerClassName = "BusinessCircle")]
public class BusinessCircleUserAuthorizationInfoResponse : WechatPayResponse
{
    /// <summary>【顾客OpenID】 顾客授权时使用的小程序上的OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【授权状态】 顾客授权商圈积分结果，1."UNAUTHORIZED":未授权 ，2."AUTHORIZED":已授权 ，3."DEAUTHORIZED":已取消授权（官方字段路径 <c>authorize_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorize_state")]
    public string? AuthorizeState { get; set; }

    /// <summary>【授权时间】 顾客成功授权商圈积分的时间（官方字段路径 <c>authorize_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorize_time")]
    public string? AuthorizeTime { get; set; }

    /// <summary>【取消授权时间】 顾客关闭授权商圈积分的时间（官方字段路径 <c>deauthorize_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deauthorize_time")]
    public string? DeauthorizeTime { get; set; }

}
