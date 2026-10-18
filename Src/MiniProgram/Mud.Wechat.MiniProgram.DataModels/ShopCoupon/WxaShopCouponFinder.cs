// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 推广视频号信息（官方 <c>coupon.coupon_info.promote_info.finder</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方参数表只列 <c>nickname</c> 一支</b>（必填）⇒ 不得凭视频号其它接口的 <c>finderuser</c> / <c>exportname</c> 等字段扩列。</para>
/// <para><b>应答侧可能整体为空对象</b>（<c>get_coupon_list</c> 示例出现 <c>"finder": {}</c>）⇒ 消费侧须容忍 <see cref="Nickname"/> 缺省。</para>
/// <para><b>敏感面</b>：视频号昵称属商家侧标识信息，禁写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponFinder
{
    /// <summary>推广视频号昵称（<c>nickname</c>，官方标必填；应答里可缺省）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
