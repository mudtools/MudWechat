// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Qic;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「质检管理」域 SDK（5 端点：质检仓配置 / 送检配置模板 /
/// 打印质检码 / 绑定送检信息 / 自寄快递送检）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域定位</b>：本域是微信小店质检（QIC）服务的送检编排面，全部端点路由走 <c>/channels/ec/qic/*</c> 主干前缀。
/// </para>
/// <para>
/// <b>动词混合（照抄官方）</b>：两个查询端点（<c>config/get</c>、<c>submitconfig/get</c>）官方为 <b>GET</b>，
/// 三个操作端点（<c>code/print</c>、<c>submit</c>、<c>register_logistics</c>）官方为 <b>POST</b>。
/// </para>
/// <para>
/// <b>质检码响应为驼峰命名</b>：<c>inspect/code/print</c> 的 <c>data</c> 对象字段为 camelCase
/// （如 <c>backupDeliveryId</c>、<c>boxDTOList</c>），与小店其余端点的 snake_case 不同，字段名照抄官方。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Qic", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsQicService
{
    /// <summary>
    /// 查询质检仓配置（<c>getinspectconfig</c>，质检仓 / 收发货地址）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>质检仓配置（<c>inspect_config</c>：质检仓 id / 名称 / 地址 / 发货地址 / 退货地址）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/qic/api_getinspectconfig"/></para>
    /// <para>官方契约：<b>GET</b> + 空请求体；<b>不支持云调用</b>。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>10021600</c>（质检系统异常）/ <c>10021602</c>（小店尚未配置质检基础信息）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>；适用账号类型仅「微信小店」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/channels/ec/qic/inspect/config/get")]
    Task<ChannelsGetInspectConfigResponse> GetInspectConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询送检配置模板信息（<c>getinspectsubmitconfig</c>，绑定送检信息的前置查询）。
    /// </summary>
    /// <param name="orderId">订单 id（官方 <c>order_id</c>，URL 参数，选填；官方变更日志 2025-11-13 由 order_id_list 改为 order_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 送检配置模板（<c>submit_config</c>：快递公司列表 <c>delivery_list</c>（含快递产品与保价类型）、
    /// 质检机构列表 <c>inspect_org_list</c>、资费标准 <c>charge_url</c>）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/qic/api_getinspectsubmitconfig"/></para>
    /// <para>官方契约：<b>GET</b> + 空请求体（<c>order_id</c> 走 URL 参数）；<b>不支持云调用</b>。
    /// 质检机构类别枚举见 <see cref="ChannelsInspectOrgCategories"/>、保价支持枚举见
    /// <see cref="ChannelsInspectInsureSupports"/>、保价上限类型枚举见 <see cref="ChannelsInspectUpperLimitTypes"/>。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>10021600</c>（质检系统异常）/ <c>10021602</c>（小店尚未配置质检基础信息）/
    /// <c>10021606</c>（质检服务未开通）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>；适用账号类型仅「微信小店」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/channels/ec/qic/inspect/submitconfig/get")]
    Task<ChannelsGetInspectSubmitConfigResponse> GetInspectSubmitConfigAsync(
        [Query("order_id")] string? orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 打印质检码（<c>printinspectcode</c>，绑定完送检信息后打印）。
    /// </summary>
    /// <param name="request">打印请求（<c>order_id</c> 必填，订单 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>质检码详情（<c>data</c>：质检码 / 质检仓与机构信息 / 快递信息 / 商品信息 / 打印模板数据 <c>printInfo</c> 等）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/qic/api_printinspectcode"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。响应 <c>data</c> 为 camelCase 字段命名（官方照录）。</para>
    /// <para>
    /// 官方业务限制：①绑定完送检信息后可通过本接口打印微信小店的质检码；②质检面单打印新方案——
    /// 只需把 <c>printInfo</c> 字段传入打印控件即可进行打印（base64 编码的模板数据与模板 html 地址）。
    /// </para>
    /// <para>官方错误码：<c>10021600</c>（质检系统异常）/ <c>10021601</c>（质检单不存在）/
    /// <c>10021603</c>（查询订单详情失败）/ <c>10021604</c>（查询商品信息失败）/ <c>10021609</c>（未录入送检信息）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>；适用账号类型仅「微信小店」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/qic/inspect/code/print")]
    Task<ChannelsPrintInspectCodeResponse> PrintInspectCodeAsync(
        [Body] ChannelsPrintInspectCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 绑定送检信息（<c>submitinspectinfo</c>，快递送检的送检编排）。
    /// </summary>
    /// <param name="request">绑定请求（<c>order_id</c>/<c>inspect_info</c> 必填；字段约束见 <see cref="ChannelsInspectInfo"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/qic/api_submitinspectinfo"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。售后拦截策略枚举见 <see cref="ChannelsInspectRefundIntercepts"/>。</para>
    /// <para>
    /// 官方业务限制：①需保价时 <c>express_insure</c> 为 true 且保价金额不能高于保价产品金额上限；
    /// ②珠宝 / 沉香质检机构（<c>inspect_org_id</c> / <c>agarwood_inspect_org_id</c>）按送检配置模板的
    /// <c>org_category</c> 至少填一个不为空；③备注限 32 个 utf8 字符且不可含材质信息；
    /// ④已入库质检单不支持修改送检信息；⑤更新送检信息不可太频繁。
    /// </para>
    /// <para>
    /// 官方错误码（节选）：<c>10021605</c>（当前质检单已入库，不支持修改送检信息）/ <c>10021606</c>（质检服务未开通）/
    /// <c>10021607</c>（质检服务已暂停，账户余额低于阈值）/ <c>10021610</c>（当前订单 id 不属于当前商家 appid）/
    /// <c>10021611</c>（保价金额超出快递产品保价上限）/ <c>10021612</c>（发货快递产品不支持保价）/
    /// <c>10021615</c>（当前质检仓尚未开通任何质检机构）/ <c>10021617</c>（保价金额不能大于订单金额）/
    /// <c>10021619</c>（备注超出字数限制）/ <c>10021620</c>（不可备注材质信息）/ <c>10021621</c>（更新送检信息太频繁）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>；适用账号类型仅「微信小店」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/qic/inspect/submit")]
    Task<ChannelsResponse> SubmitInspectInfoAsync(
        [Body] ChannelsSubmitInspectRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 自寄快递送检（<c>registerlogistics</c>，登记/修改自寄快递信息）。
    /// </summary>
    /// <param name="request">登记请求（<c>order_id_list</c>/<c>logistics_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/qic/api_registerlogistics"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。发货方式枚举见 <see cref="ChannelsInspectDeliveryTypes"/>（本接口固定 1 自寄快递发货）。</para>
    /// <para><b>官方业务限制：可通过该接口登记/修改自寄快递信息，最多可修改三次</b>。</para>
    /// <para>
    /// 官方错误码（节选）：<c>10021605</c>（当前质检单已入库，不支持修改送检信息）/ <c>10021622</c>（物流单号非法）/
    /// <c>10021623</c>（超出修改次数上限）/ <c>10021624</c>（当前订单质检状态不允许修改物流）/
    /// <c>10021625</c>（物流信息修改部分成功，可稍后重试）/ <c>10021626</c>（快递公司跟物流单号不匹配）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>；适用账号类型仅「微信小店」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/qic/inspect/register_logistics")]
    Task<ChannelsResponse> RegisterInspectLogisticsAsync(
        [Body] ChannelsRegisterInspectLogisticsRequest request,
        CancellationToken cancellationToken = default);
}
