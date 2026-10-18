// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>车主服务</b>域（直连商户侧）3 支端点：入场通知 / 扣款申请 / 用户状态查询。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：
/// 用户入场通知 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985272"/>、
/// 停车扣款申请 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985331"/>、
/// 用户状态查询 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986415"/>。</para>
/// <para><b>三支均不需证书</b>：三页「是否需要证书」栏皆无「是」的记载 ⇒ 走声明式管道
/// （普通域名 + 报文内 <c>sign</c>），不落证书收发器。</para>
/// <para><b>域名是 <c>api.mch</c></b>：三页接口地址栏均为 <c>https://api.mch.weixin.qq.com/vehicle/…</c>，
/// 全文无 <c>api2.mch</c>。</para>
/// <para><b><c>sign_type</c> 三支同取 HMAC-SHA256</b>：三页均写「签名类型，默认为HMAC-SHA256」且不列 MD5 备选
/// ⇒ 请求 DTO 的默认值即契约面（见各 DTO 的备注），清空必 <c>SIGN_ERROR</c>。</para>
/// <para><b>本域是「先放行后扣款」的信用支付形态</b>：入场通知注册车牌状态、扣款申请只<b>受理</b>，
/// 结果一律以异步通知或订单查询为准 ⇒ 不得据同步应答的 <c>result_code</c> 判定扣款成功。</para>
/// <para><b>服务商侧另成一族</b>：<c>/vehicle/partnerpay/*</c> 三支见
/// <c>IWechatPayV2PartnerVehicleService</c>；两族的参数表<b>并非同形</b>
/// （子商户字段、<c>trade_scene</c> 枚举档数、返回表成员都有差异），不得复用 DTO。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "VehicleV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2VehicleService
{
    /// <summary>
    /// 车主服务用户入场通知（APIv2，直连商户侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985272"/>。
    /// </summary>
    /// <param name="request">入场通知请求体，见 <see cref="V2VehicleNotificationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>注册结果，见 <see cref="V2VehicleNotificationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/vehicle/pay/notification</c>；XML 信封。</para>
    /// <para><b>微信侧不下发通知</b>（官方原文）：「调用本接口微信侧不会向用户下发入场通知消息，
    /// 消息由商户下发」⇒ 本支的作用是<b>注册车牌的状态变更通知</b>（车场停车场景必须调用），不是发消息。</para>
    /// <para><b><c>scene_info</c> 是 JSON 字符串袋</b>：场景子字段整体作为一段 JSON 放进该节点，
    /// 官方参数表把子字段平铺缩进只是排版 ⇒ 误当 XML 顶层字段上传即 <c>PARAM_ERROR</c>。</para>
    /// </remarks>
    [Post("/vehicle/pay/notification")]
    Task<V2VehicleNotificationResponse> VehicleNotificationAsync(
        V2VehicleNotificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 车主服务扣款申请（APIv2，直连商户侧；停车/加油/高速/桥隧四场景）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985331"/>。
    /// </summary>
    /// <param name="request">扣款申请请求体，见 <see cref="V2VehiclePayApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果（<b>不含</b>金额与交易状态），见 <see cref="V2VehiclePayApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/vehicle/pay/payapply</c>；XML 信封；
    /// <c>total_fee</c> 单位为分。</para>
    /// <para><b>受理 ≠ 扣款成功</b>：应答只有通用 10 行、没有任何金额或 <c>trade_state</c> ⇒
    /// 扣款结果必须走异步通知或订单查询，据此放行车辆会资损。</para>
    /// <para><b>场景时限与一致性（官方硬性）</b>：高速 ETC 通道需在 <b>48 小时</b>内把交易送达
    /// （超时 <c>TRADE_TIME_INVALID</c>）；<c>channel_type</c> 必须与车牌的通道标识一致
    /// （不一致 <c>CHANNEL_TYPE_INVALID</c>）；车牌<b>只支持蓝牌、绿牌</b>。</para>
    /// <para><b>重试口径</b>：本族 17 个错误码中 <c>SYSTEMERROR</c> 官方给出 10 次退避序列
    /// 30s,60s,60s,120s,120s,180s,180s,360s,360s,600s ⇒ 重试策略照此实现，不做即时连续重试。</para>
    /// <para><b>错误码形态</b>：本页签名错误写的是下划线形态 <c>SIGN_ERROR</c>
    /// （基础支付各族写 <c>SIGNERROR</c>），判错时两种拼写都要认。</para>
    /// </remarks>
    [Post("/vehicle/pay/payapply")]
    Task<V2VehiclePayApplyResponse> VehiclePayApplyAsync(
        V2VehiclePayApplyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 车主服务用户状态查询（APIv2，直连商户侧）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986415"/>。
    /// </summary>
    /// <param name="request">状态查询请求体，见 <see cref="V2VehicleStateQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户授权/欠费状态与引导跳转路径，见 <see cref="V2VehicleStateQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/vehicle/pay/querystate</c>；
    /// <c>openid</c> 与 <c>plate_number</c> <b>二选一</b>（官方各自标「必传其一」）。</para>
    /// <para><b>状态四档</b>：<c>NORMAL</c> / <c>PAUSED</c> / <c>OVERDUE</c> / <c>UNAUTHORIZED</c>；
    /// 后两档官方同时回 <c>path</c> 引导入口，商户须让用户跳转授权/补缴后才能继续扣款。
    /// 注意入场通知那一支的 <c>user_status</c> 只有两档（<c>NORMAL</c>/<c>BLOCKED</c>），
    /// 两页<b>不同名同义</b>，勿混用枚举。</para>
    /// <para><b><c>jump_scene</c> 选填</b>：不传默认小程序，另支持 <c>APP</c> 与公众号 <c>H5</c>
    /// ⇒ 决定 <c>path</c> 的形态，跨端复用时要显式传。</para>
    /// </remarks>
    [Post("/vehicle/pay/querystate")]
    Task<V2VehicleStateQueryResponse> VehicleStateQueryAsync(
        V2VehicleStateQueryRequest request, CancellationToken cancellationToken = default);
}
