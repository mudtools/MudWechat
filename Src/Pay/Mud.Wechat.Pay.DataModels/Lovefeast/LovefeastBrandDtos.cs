// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Lovefeast;

/// <summary>查询爱心餐品牌信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472863"/></summary>
[HttpJsonSerializable(SerializerClassName = "Lovefeast")]
public class LovefeastBrandInfoResponse : WechatPayResponse
{
    /// <summary>【捐赠数量】 用户在该品牌下捐赠的总次数（官方字段路径 <c>donate_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("donate_count")]
    public long? DonateCount { get; set; }

    /// <summary>【供餐数量】 仅供餐品牌返回，该品牌为环卫工人供餐总量（官方字段路径 <c>consume_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("consume_count")]
    public long? ConsumeCount { get; set; }
}
