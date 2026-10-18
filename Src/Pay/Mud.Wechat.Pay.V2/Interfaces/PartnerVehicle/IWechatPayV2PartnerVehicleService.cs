// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>车主服务（服务商侧）</b> 3 支端点：入场通知 / 扣款申请 / 用户状态查询。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 服务商侧，2026-10-18 逐页核验）：
/// 用户入场通知 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988204"/>、
/// 停车扣款申请 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988205"/>、
/// 用户状态查询 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988212"/>。</para>
/// <para><b>三支均不需证书、挂在 <c>api.mch</c></b>：三页接口地址栏为
/// <c>https://api.mch.weixin.qq.com/vehicle/partnerpay/…</c>，无证书要求栏 ⇒ 与直连族同走声明式管道。</para>
/// <para><b>服务商族不是直连族加 <c>sub_*</c></b>：三页与
/// <c>IWechatPayV2VehicleService</c> 的对应页在<b>返回表成员</b>（入场通知应答不列 <c>sub_appid</c>/
/// <c>sub_mch_id</c>，扣款申请应答却列）、<b><c>trade_scene</c> 档数</b>（状态查询页只有四档、无 <c>BRIDGE</c>）、
/// <b>请求字段集</b>（扣款申请无 <c>sub_openid</c>）三处都不同形 ⇒ DTO 各自成形、禁止共用，
/// 差异已在各 DTO 备注逐处留痕。</para>
/// <para><b>官方页无请求示例</b>：服务商入场通知页<b>不给</b> XML 示例、且把场景子字段混排进顶层表 ⇒
/// <c>scene_info</c> 的字符串袋口径沿用直连页（同一字段的官方示例只出现在直连页）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerVehicleV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2PartnerVehicleService
{
    /// <summary>
    /// 车主服务用户入场通知（APIv2，服务商侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988204"/>。
    /// </summary>
    /// <param name="request">入场通知请求体，见 <see cref="V2PartnerVehicleNotificationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>注册结果，见 <see cref="V2PartnerVehicleNotificationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/vehicle/partnerpay/notification</c>；
    /// <c>sub_appid</c> 与 <c>sub_mch_id</c> <b>必填</b>；顶层另有选填
    /// <c>openid</c>／<c>sub_openid</c>／<c>space_number</c>。</para>
    /// <para><b>应答不列子商户字段</b>：官方本支返回表<b>不列</b> <c>sub_appid</c>/<c>sub_mch_id</c>
    /// （同族另两支都列）⇒ 照抄官方，不按「服务商统一形状」补齐。</para>
    /// <para><b>微信侧不下发通知</b>：与直连页同口径，消息由商户自行下发，本支注册的是车牌状态变更通知。</para>
    /// </remarks>
    [Post("/vehicle/partnerpay/notification")]
    Task<V2PartnerVehicleNotificationResponse> PartnerVehicleNotificationAsync(
        V2PartnerVehicleNotificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 车主服务扣款申请（APIv2，服务商侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988205"/>。
    /// </summary>
    /// <param name="request">扣款申请请求体，见 <see cref="V2PartnerVehiclePayApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果（<b>不含</b>金额与交易状态），见 <see cref="V2PartnerVehiclePayApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/vehicle/partnerpay/payapply</c>；
    /// 请求面<b>没有</b> <c>sub_openid</c>（只有 <c>openid</c>，特定场景必填）——
    /// 这是本族三支之间最不直观的差异，照抄不得补。</para>
    /// <para><b>应答比直连多两行</b>：<c>sub_appid</c>（选填）与 <c>sub_mch_id</c>（必填）；
    /// 仍然<b>没有</b>金额与 <c>trade_state</c> ⇒ 受理不等于扣款成功，结果走异步通知或订单查询。</para>
    /// <para><b><c>version</c> 描述与直连页不同</b>：本页只写「固定值 3.0」，
    /// 直连页写「3.0 车场 / 2.0 其他」⇒ 两页各按其文，服务商侧不做场景分档。</para>
    /// </remarks>
    [Post("/vehicle/partnerpay/payapply")]
    Task<V2PartnerVehiclePayApplyResponse> PartnerVehiclePayApplyAsync(
        V2PartnerVehiclePayApplyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 车主服务用户状态查询（APIv2，服务商侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988212"/>。
    /// </summary>
    /// <param name="request">状态查询请求体，见 <see cref="V2PartnerVehicleStateQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户授权/欠费状态与引导跳转路径，见 <see cref="V2PartnerVehicleStateQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/vehicle/partnerpay/querystate</c>；
    /// <c>openid</c>／<c>sub_openid</c>／<c>plate_number</c> <b>三选一</b>
    /// （直连页只有两个字段、标「二选一」）。</para>
    /// <para><b><c>trade_scene</c> 少一档</b>：本页枚举<b>不含</b> <c>BRIDGE</c>，与同族另两支不一致 ⇒
    /// 跨场景复用时不能拿扣款申请的场景枚举直接传进来（官方跨页矛盾，已留痕）。</para>
    /// <para><b>按 <c>openid</c> 查只返回主动缴纳状态</b>：官方注「传入 openid 时只返回 <c>PROACTIVE</c>」⇒
    /// 想拿车牌维度的完整状态必须传 <c>plate_number</c>。</para>
    /// <para><b>错误码多一支</b>：本页 5 个错误码，比直连页多 <c>VEHICLE_NOT_EXIST</c>（车辆不存在）。</para>
    /// </remarks>
    [Post("/vehicle/partnerpay/querystate")]
    Task<V2PartnerVehicleStateQueryResponse> PartnerVehicleStateQueryAsync(
        V2PartnerVehicleStateQueryRequest request, CancellationToken cancellationToken = default);
}
