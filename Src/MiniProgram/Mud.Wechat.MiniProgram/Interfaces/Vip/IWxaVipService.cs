// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「会员服务」域 SDK（6 端点：会员信息增删改查 + 会员列表 + 已关联小店列表，
/// 官方「小程序会员服务」分组 → 小程序相关）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档与归属裁决</b>：官方页面位于小店文档目录
/// （<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/</c>），
/// 但官方原文明确「<b>小程序 API 请使用小程序身份，即小程序的 access_token 调用</b>」，
/// 适用范围为「小程序」账号类型（2026-10-12 逐页核验）⇒ 按小店线设计方案 §4.1 结论 A
/// 归小程序线；小店线侧由 CH-V1 锁定「Channels 程序集不得声明 <c>/wxa/vip/*</c> 路由」，
/// 两守卫合起来即该路由族的归属唯一性闸门。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c> ——小程序与公众号<b>同平台、同令牌域</b>，不新增令牌类型常量
/// （新增会让 <c>MpTokenManagerRegistry</c> 单槽 <c>Resolve</c> 返回 <c>null</c> ⇒
/// errcode 令牌自愈<b>静默失效</b>）。
/// </para>
/// <para>
/// <b>第三方平台代调用</b>：官方支持第三方平台代微信小店商家调用（权限集 id <b>185</b>，
/// 以 <c>authorizer_access_token</c> 代调用）；本 SDK 仍只声明小程序应用级
/// <c>access_token</c>（Query），第三方代调用形态由宿主以小程序应用配置供给令牌，不引入新令牌链。
/// </para>
/// <para>
/// <b>调用边界（官方逐页共注）</b>：6 端点均为<b>服务器端调用</b>（不可在小端直接调用）；
/// <b>不支持云调用</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vip", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaVipService
{
    /// <summary>
    /// 新增小程序会员信息。官方文档：
    /// <c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_adduserinfo</c>。
    /// </summary>
    /// <param name="request">请求体（<c>session_id</c> / <c>wxa_openid</c> / <c>info</c> 必填），见 <see cref="DataModels.Vip.WxaVipAddUserRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/vip/user/info/add</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（小程序身份）。</para>
    /// <para>
    /// <b>业务时序（官方语义）</b>：用户在小店侧发起会员注册后，小程序侧凭小店下发的
    /// <c>session_id</c> 将自有会员信息回传同步；<c>session_id</c> <b>有效期 10 分钟</b>、
    /// 由小店传递给小程序，不得持久化复用；同一用户后续变更走
    /// <see cref="UpdateUserInfoAsync"/>。
    /// </para>
    /// <para>
    /// <b>历史字段禁建模（官方变更日志）</b>：<c>receive_register_coupon</c> 已从官方参数表
    /// <b>删除</b>，不得建模（2026-10-12 核验）。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）/ <c>2</c>（小程序用户 openid 不是小程序 appid 的用户）。</para>
    /// </remarks>
    [Post("/wxa/vip/user/info/add")]
    Task<WxaResponse> AddUserInfoAsync(
        [Body] DataModels.Vip.WxaVipAddUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新小程序会员信息。官方文档：
    /// <c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_updateuserinfo</c>。
    /// </summary>
    /// <param name="request">请求体（<c>wxa_openid</c> / <c>info</c> 必填），见 <see cref="DataModels.Vip.WxaVipUpdateUserRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/vip/user/info/update</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（小程序身份）。</para>
    /// <para>
    /// <b>语义边界（官方原文）</b>：用户<b>已关联</b>小店会员后的<b>后续更新同步</b>
    /// （如会员等级从 1 级到 2 级）；商家调接口后，小店侧会同步展示最新会员信息；
    /// 首次建档走 <see cref="AddUserInfoAsync"/>。更新成功后官方向对应小店推送会员信息变更的事件通知。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）/ <c>2</c>（小程序用户 openid 不是小程序 appid 的用户）。</para>
    /// </remarks>
    [Post("/wxa/vip/user/info/update")]
    Task<WxaResponse> UpdateUserInfoAsync(
        [Body] DataModels.Vip.WxaVipUpdateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序会员信息。官方文档：
    /// <c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserinfo</c>。
    /// </summary>
    /// <param name="request">请求体（<c>wxa_openid</c> 必填），见 <see cref="DataModels.Vip.WxaVipOpenidRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会员信息（<c>info</c>：会员身份 / 外部会员 id / 已关联小店列表），见 <see cref="DataModels.Vip.WxaVipUserInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/vip/user/info/get</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（小程序身份）。</para>
    /// <para>
    /// <b>响应字段集</b>：<c>info</c> 含 <c>identity</c> / <c>outer_userid</c> /
    /// <c>related_shop_appid</c>（用户当前已关联的小店 appid 列表）——官方参数表
    /// <b>不含</b> <c>wxa_openid</c>（该字段仅会员列表元素携带，见 <see cref="GetUserListAsync"/>）。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）/ <c>2</c>（小程序用户 openid 不是小程序 appid 的用户）。</para>
    /// </remarks>
    [Post("/wxa/vip/user/info/get")]
    Task<DataModels.Vip.WxaVipUserInfoResponse> GetUserInfoAsync(
        [Body] DataModels.Vip.WxaVipOpenidRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序会员列表。官方文档：
    /// <c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserlist</c>。
    /// </summary>
    /// <param name="request">请求体（<c>offset</c> / <c>limit</c> 官方均必填），见 <see cref="DataModels.Vip.WxaVipGetUserListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会员列表（<c>list[]</c>：会员身份 / 外部会员 id / 小程序 openid / 已关联小店列表），见 <see cref="DataModels.Vip.WxaVipGetUserListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/vip/user/list/get</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（小程序身份）。</para>
    /// <para>
    /// <b>分页契约（官方原文）</b>：<c>offset</c>（偏移量）与 <c>limit</c>（本次读取数量）均<b>必填</b>；
    /// 列表元素字段集为 <c>identity</c> / <c>outer_userid</c> / <c>wxa_openid</c> /
    /// <c>related_shop_appid</c> —— 比单查响应 <c>info</c> <b>多</b> <c>wxa_openid</c>（官方原文如此，DTO 分立建模）。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）。</para>
    /// </remarks>
    [Post("/wxa/vip/user/list/get")]
    Task<DataModels.Vip.WxaVipGetUserListResponse> GetUserListAsync(
        [Body] DataModels.Vip.WxaVipGetUserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除小程序会员信息。官方文档：
    /// <c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_deluserinfo</c>。
    /// </summary>
    /// <param name="request">请求体（<c>wxa_openid</c> 必填；与 <see cref="GetUserInfoAsync"/> 共用 <see cref="DataModels.Vip.WxaVipOpenidRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/vip/user/info/delete</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（小程序身份）。</para>
    /// <para>
    /// <b>同步语义（官方原文）</b>：删除后官方同步删除小店侧的会员信息（并推送相关事件通知）
    /// ——宿主侧须按幂等语义设计（会员删除在小程序与小店两侧联动）。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）/ <c>2</c>（小程序用户 openid 不是小程序 appid 的用户）。</para>
    /// </remarks>
    [Post("/wxa/vip/user/info/delete")]
    Task<WxaResponse> DeleteUserInfoAsync(
        [Body] DataModels.Vip.WxaVipOpenidRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序已关联小店列表。官方文档：
    /// <c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getshoplist</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已关联小店列表（<c>list[]</c>：小店 appid），见 <see cref="DataModels.Vip.WxaVipGetShopListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/vip/shop/list/get</c>；Query 携带 <c>access_token</c>（小程序身份）。</para>
    /// <para>
    /// <b>空请求体规则</b>：官方 Request Payload 为<b>空</b>（请求示例 <c>{}</c>，参数表无任何字段）
    /// ——SDK 不声明请求体参数，不得人为造请求体 DTO。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）/ <c>2</c>（没有小店绑定记录）。</para>
    /// </remarks>
    [Post("/wxa/vip/shop/list/get")]
    Task<DataModels.Vip.WxaVipGetShopListResponse> GetShopListAsync(
        CancellationToken cancellationToken = default);
}
