// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「发货信息与订单 · 设置与变更申请」域 SDK（2 端点：消息跳转路径设置 /
/// 小程序交易类型变更申请；官方 order_shipping 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两支的共同点不是「配置」而是「账号级、非幂等的申报动作」</b>：跳转路径是账号级唯一值（重复设置即覆盖），
/// 交易类型变更则是<b>一个账号只能申请一次</b>的审批类端点 ⇒ 两者都不适合放进重试管道。
/// </para>
/// <para>
/// <b>第三方开放面在本接口内也不统一</b>：<see cref="SetMsgJumpPathAsync"/> 官方明写
/// 「本接口不支持第三方平台调用」，<see cref="ApplyTradeTypeChangeAsync"/> 支持以
/// <c>authorizer_access_token</c> 代调用（权限集 142）。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para><b>调用限制</b>：两端点官方正文均未公布端点级频率数字；「材料最多 10 个、其中视频最多 3 个」是规模限制，不得当频率解读。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OrderShipping", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaOrderSettingService
{
    /// <summary>
    /// 消息跳转路径设置（<c>POST /wxa/sec/order/set_msg_jump_path</c>，云调用 <c>wxa.sec.order.setMsgJumpPath</c>，<b>不支持第三方平台调用</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setmsgjumppath.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>path</c> 一字段，必填），见 <see cref="WxaOrderMsgJumpPathRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；前提是<b>已接入平台确认收货组件</b>，作用是让用户点击发货 / 确认收货消息直接进到自己的小程序页面。</para>
    /// <para>
    /// <b>成功响应不代表生效</b>（官方注意事项）：设置为空路径或小程序中<b>不存在</b>的路径时调用照样成功，
    /// 用户仍跳平台默认确认收货页 ⇒ 宿主须自行校验页面路径存在性，不能以 <c>errcode=0</c> 当作落地证据。
    /// </para>
    /// <para>
    /// <b>平台会往路径上追加 Query 参数</b>（官方原文）：<c>transaction_id</c>、<c>merchant_id</c>、
    /// <c>merchant_trade_no</c>，存在二级商户号时再加 <c>sub_merchant_id</c> ⇒ 自定义 Query <b>必须避开这四个键名</b>，
    /// 小程序侧用 <c>onLaunch</c> 等方式读取。
    /// </para>
    /// <para><b>官方接口英文名拼写为 <c>setMsgJumPpath</c></b>（页面「接口英文名」字段原文），与路由及云调用方法名 <c>setMsgJumpPath</c> 不一致 ⇒ 本仓按路由与云调用名落地，英文名差异仅留档。</para>
    /// <para><b>本端点官方只有通用错误码 <c>-1</c></b>（错误码表恰 1 行）⇒ 无域内码可套。</para>
    /// </remarks>
    [Post("/wxa/sec/order/set_msg_jump_path")]
    Task<WxaResponse> SetMsgJumpPathAsync(
        [Body] WxaOrderMsgJumpPathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 小程序交易类型变更申请（<c>POST /wxa/sec/order/setwxatradetypecgi</c>，<b>不支持云调用</b>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setwxatradetypecgi.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>trade_type</c> / <c>material_list</c> / <c>reason</c> 三支全必填），见 <see cref="WxaOrderTradeTypeChangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<c>trade_type</c> 取值见 <see cref="WxaOrderTradeTypeValues"/>（1 综合类 / 2 实物电商类 / 3 线下服务类 / 4 在线服务类）。</para>
    /// <para><b>一个账号只能申请一次</b>（官方注意事项原文）：重复提交返回 <c>268486048</c>，需多次申请须联系小程序客服 ⇒ 本端点<b>不得</b>放进自动重试链路。</para>
    /// <para><b>材料须先上传</b>（官方原文）：图片与视频都要先经「新增临时素材」接口上传换 <c>media_id</c> 再提交；规模上限为最多 10 个材料、其中视频最多 3 个。</para>
    /// <para><b>本端点无查询伴侣</b>：官方 order_shipping 目录内没有「查询当前交易类型」页面 ⇒ 变更结果只能通过小程序后台或客服确认，宿主不要指望轮询本域端点得到结论。</para>
    /// <para><b>路由名照官方原文</b>（<c>setwxatradetypecgi</c> 全小写连写、含历史后缀 <c>cgi</c>），不得写成下划线形态；守卫 OS1 锁定。</para>
    /// </remarks>
    [Post("/wxa/sec/order/setwxatradetypecgi")]
    Task<WxaResponse> ApplyTradeTypeChangeAsync(
        [Body] WxaOrderTradeTypeChangeRequest request,
        CancellationToken cancellationToken = default);
}
