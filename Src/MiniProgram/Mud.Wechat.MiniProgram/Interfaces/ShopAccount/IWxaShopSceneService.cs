// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 支付校验场景」域 SDK（1 端点：检查场景值是否在支付校验范围内；
/// 官方 minishopopencomponent2 分区 → order 子目录，路由前缀是 <c>/shop/scene/*</c> 而非 <c>/shop/order/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 官方本页无权限集编号、无第三方代调用与云调用声明（空缺留档）。
/// </para>
/// <para>
/// <b>路由前缀与文档目录不同源</b>（本域唯一的路由级反直觉点）：官方页面位于 <c>API/order/check_scene.html</c>
/// （订单接口分区），而请求地址是 <c>https://api.weixin.qq.com/shop/scene/check</c> ⇒
/// 落位与守卫一律以<b>页面正文的请求地址</b>为权威（作业书 §1 判定口径），不得按目录前缀写成 <c>/shop/order/*</c>。
/// </para>
/// <para>
/// <b>官方本页标注待废弃</b>（页面标题「检查场景值是否在支付校验范围内(待废弃）」、正文首行
/// 「接口待废弃，请使用下单前置校验基础库接口」）：页面仍 200 + 有完整「http 请求方式 / 请求地址」小节 +
/// 在现行目录树内 ⇒ 按作业书 §1 判定为<b>契约在册</b>并照官方落地，同时在注释里明示弃用走向，
/// 新接入优先官方指引的基础库下单前置校验。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopSceneService
{
    /// <summary>
    /// 检查场景值是否在支付校验范围内（<b>官方标注待废弃</b>，<c>POST /shop/scene/check</c>；
    /// 权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/check_scene.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>scene</c> 一支，必填），见 <see cref="WxaShopSceneCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否需要支付校验（顶层 <c>is_matched</c>，条件性 <c>requiredFundType</c>），见 <see cref="WxaShopSceneCheckResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b>业务语义（官方原文）</b>：「微信后台会对符合支付校验范围内的场景值下的收银台进行支付
    /// （ticket / 订单信息）校验」⇒ 本端点只回答「该场景值是否会触发支付校验」，不校验订单本身。</para>
    /// <para><b>回包表里有、示例里没有的字段</b>：<c>requiredFundType</c>（官方原文「require_order = 1 时生效」，
    /// 取值 <c>0</c> 非二级商户号订单 / <c>1</c> 二级商户号订单 / <c>2</c> 两种方式皆可，官方注「后续只会存在 1」）
    /// ⇒ 宿主须容忍该键缺省；本页请求表<b>没有</b> <c>require_order</c> 这支参数，属官方正文的内部引用悬空，照录留档。</para>
    /// <para><b><c>requiredFundType</c> 是本域唯一驼峰键</b>（其余全为 snake_case）⇒ 键名照官方原文钉死，
    /// 不得「顺手」改成下划线形态，守卫 SHOPA5 锁定。</para>
    /// <para><b><c>is_matched</c> 官方示例是数值 <c>1</c></b>（类型列写 enum）⇒ 建为可空数值、不改布尔，保持线格式一致。</para>
    /// </remarks>
    [Post("/shop/scene/check")]
    Task<WxaShopSceneCheckResponse> CheckSceneAsync(
        [Body] WxaShopSceneCheckRequest request,
        CancellationToken cancellationToken = default);
}
