// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「自定义菜单」域 SDK（7 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 自定义菜单。各端点 remarks 中的深链均为逐页核验过的真实页面。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>菜单规模硬上限</b>：最多 <b>3 个一级菜单</b>，每个一级最多 <b>5 个二级菜单</b>；
/// 标题一级 ≤ 16 字节（显示层 4 个汉字）、二级 ≤ 60 字节（显示层 8 个汉字，超出以 <c>...</c> 代替）。</item>
/// <item><b>个性化菜单的每日次数上限（官方明确声明）</b>：普通公众号——新增 <b>2000 次/日</b>、
/// 删除 <b>2000 次/日</b>、测试匹配结果 <b>20000 次/日</b>。</item>
/// <item><b>级联删除 / 前置依赖</b>：创建个性化菜单<b>必须先创建默认菜单</b>；
/// <b>删除默认菜单会连带删除全部个性化菜单</b>。</item>
/// <item><b>无编辑 API（官方设计决定）</b>：个性化菜单更新会被覆盖（按发布顺序倒序匹配），
/// 官方<b>不提供</b>个性化菜单编辑接口 ⇒ 更新须<b>完整重新发布一轮</b>。</item>
/// <item><b>安全约束</b>：出于安全考虑，一个公众号的所有个性化菜单<b>最多只能跳转到 3 个域名</b>下的链接。</item>
/// <item><b>标签匹配语义</b>：用户身上标签超过 1 个时，<b>以最后打上的标签</b>为匹配依据。</item>
/// <item><b>字符集陷阱</b>：官方 <c>40033</c> 明确「请求包含 <c>\uxxxx</c> 格式字符会导致创建失败」⇒
/// 本 SDK 已在注册期把序列化编码器放宽为不转义非 ASCII（见 <c>AddMpApp</c> 注册期 remarks），
/// 并有用例锁定「中文菜单名不得出现 <c>\u</c> 转义」。</item>
/// <item><b>账号资格</b>：除 <c>getCurrentSelfmenuInfo</c> 外，官方均标注「仅认证」；
/// <c>getCurrentSelfmenuInfo</c> 官方原文为「认证 / 未认证的服务号 / 公众号，以及接口测试号，均拥有该接口权限」。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入 <c>access_token</c>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Menu", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpMenuService
{
    /// <summary>
    /// 创建自定义菜单。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_createcustommenu.html"/>
    /// （官方接口英文名 <c>createCustomMenu</c>）。
    /// </summary>
    /// <param name="request">菜单结构（一级 1~3 个，每个一级最多 5 个二级）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方「注意事项」原文：①最多 3 个一级菜单，每个一级最多 5 个二级；②一级最多 4 个汉字、二级最多 8 个汉字
    /// （超出以 <c>...</c> 代替）；③刷新策略——用户进入会话页 / profile 页时，若上次拉取菜单已超过
    /// <b>5 分钟</b>则重新拉取，有更新才刷新客户端菜单。
    /// </para>
    /// <para>
    /// 官方错误码：<c>0</c> / <c>40018</c>（按钮名字长度），以及通用「按钮/子按钮」系列
    /// <c>40016</c>/<c>40017</c>/<c>40019</c>/<c>40020</c>/<c>40023</c>/<c>40024</c>/<c>40027</c>/
    /// <c>40033</c>（含 <c>\uxxxx</c> 非法字符）/<c>40054</c>/<c>40055</c>（url 域名非法）。
    /// </para>
    /// <para>第三方调用权限集 id 为 <c>15</c>；接口变更日志 2025-11-25 更新了 <c>appid</c> 字段描述。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/menu/create")]
    Task<MpResponse> CreateMenuAsync(
        [Body] MpCreateMenuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取自定义菜单配置。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_getmenu.html"/>
    /// （官方接口英文名 <c>getMenu</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>默认菜单（<c>menu</c>）与个性化菜单列表（<c>conditionalmenu</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET 且无请求体</b>（勿为「统一风格」加请求体）。</para>
    /// <para>官方「注意事项」原文：「在设置了个性化菜单后，使用本自定义菜单查询接口可以获取默认菜单和全部个性化菜单信息」。</para>
    /// <para>
    /// <b>与 <see cref="GetCurrentSelfMenuInfoAsync"/> 的能力边界</b>：本接口<b>只能查到用 API 设置的菜单</b>；
    /// 要连官网设置的菜单一起查，须用后者。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/menu/get")]
    Task<MpGetMenuResponse> GetMenuAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除当前使用的自定义菜单。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_deletemenu.html"/>
    /// （官方接口英文名 <c>deleteMenu</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方「注意事项」原文</b>：「在个性化菜单时，调用此接口会删除<b>默认菜单及全部个性化菜单</b>」
    /// ⇒ 这是一条「删除即级联」的隐藏语义，调用方须显式知情。
    /// </para>
    /// <para>官方契约：<b>GET 且无请求体</b>；错误码 <c>-1</c>/<c>0</c>/<c>40001</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/menu/delete")]
    Task<MpResponse> DeleteMenuAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建个性化菜单。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_addconditionalmenu.html"/>
    /// （官方接口英文名 <c>addConditionalMenu</c>）。
    /// </summary>
    /// <param name="request">个性化菜单（<c>button</c> + <c>matchrule</c>，后者至少一个非空字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>menuid</c>（删除个性化菜单时须回传）。</returns>
    /// <remarks>
    /// <para><b>前置依赖</b>：创建个性化菜单之前<b>必须先创建默认菜单</b>；删除默认菜单会连带删除全部个性化菜单。</para>
    /// <para>
    /// <b>每日次数上限（官方明确）</b>：普通公众号个性化菜单<b>新增 2000 次/日</b>（删除同为 2000 次/日，
    /// 测试匹配结果为 20000 次/日）⇒ 调用方须自行控频（本 SDK 不做本地令牌桶，避免与官方口径双重计数）。
    /// </para>
    /// <para>
    /// <b>客户端版本门槛</b>：个性化菜单要求 iPhone <b>6.2.2</b> / Android <b>6.2.4</b> 以上，暂不支持其他版本。
    /// </para>
    /// <para>
    /// <b>域名上限</b>：一个公众号的所有个性化菜单最多只能跳转到 <b>3 个域名</b>下的链接（安全约束）。
    /// </para>
    /// <para>
    /// <b>更新语义</b>：个性化菜单的更新会被覆盖（按发布顺序倒序匹配，命中即返回）；
    /// 官方<b>不提供</b>编辑 API ⇒ 更新须完整重新发布一轮。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40007</c> / <c>40016</c> / <c>40017</c> / <c>40018</c> /
    /// <c>40019</c> / <c>40020</c> / <c>40023</c> / <c>40024</c> / <c>40027</c> / <c>40033</c> / <c>40054</c> /
    /// <c>40055</c> / <c>53600</c> / <c>65320</c>（匹配规则含隐私字段）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/menu/addconditional")]
    Task<MpAddConditionalMenuResponse> AddConditionalMenuAsync(
        [Body] MpConditionalMenu request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除个性化菜单。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_deleteconditionalmenu.html"/>
    /// （官方接口英文名 <c>deleteConditionalMenu</c>）。
    /// </summary>
    /// <param name="request">待删除的个性化菜单标识（<c>menuid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方「注意事项」原文：①个性化菜单要求客户端 iPhone 6.2.2 / Android 6.2.4 以上；
    /// ②<b>每日删除限制 2000 次</b>。
    /// </para>
    /// <para>官方错误码：<c>0</c>（<c>ok</c>：从不正常变成正常，或本来就正常）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/menu/delconditional")]
    Task<MpResponse> DeleteConditionalMenuAsync(
        [Body] MpDeleteConditionalMenuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试个性化菜单匹配结果。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_trymatchmenu.html"/>
    /// （官方接口英文名 <c>tryMatchMenu</c>）。
    /// </summary>
    /// <param name="request">待匹配用户（<c>user_id</c>：OpenID <b>或微信号</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>命中的菜单按钮数组（官方字段表<b>不含</b> <c>matchrule</c>/<c>menuid</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方「注意事项」原文：①<b>每日测试限制 20000 次</b>；②「<b>包含已废除字段的菜单也将自动失效，
    /// 不再被匹配</b>。这一点也将体现在本测试接口中」⇒ 可用本接口提前发现「已废除字段」导致的菜单失效。
    /// </para>
    /// <para><c>user_id</c> 支持 <b>OpenID 或微信号</b>（非仅 openid），调用方可直接传运营人员微信号自测。</para>
    /// <para>官方错误码：<c>0</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/menu/trymatch")]
    Task<MpTryMatchMenuResponse> TryMatchMenuAsync(
        [Body] MpTryMatchMenuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询自定义菜单信息（含官网配置）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/custommenu/api_getcurrentselfmenuinfo.html"/>
    /// （官方接口英文名 <c>getCurrentSelfmenuInfo</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>is_menu_open</c> 与 <c>selfmenu_info</c>（结构与菜单查询接口<b>不同</b>，见 DTO remarks）。</returns>
    /// <remarks>
    /// <para>
    /// <b>与菜单查询接口的能力差异（官方原文）</b>：本接口除 API 设置的菜单外，还能查看<b>通过公众平台官网
    /// （mp.weixin.qq.com）设置的菜单</b>；菜单查询接口只能查到 API 设置的菜单。
    /// </para>
    /// <para>
    /// <b>权限面更宽（官方原文）</b>：认证 / 未认证的服务号 / 公众号，以及接口测试号，<b>均拥有该接口权限</b>
    /// （与菜单域其余端点的「仅认证」不同）。
    /// </para>
    /// <para>
    /// 官方「注意事项」还指出：返回的图片 / 语音 / 视频为<b>临时素材</b>（每次获取都不同、3 天内有效，
    /// 经素材管理-获取临时素材接口获取）；返回的图文消息为<b>永久素材</b>。
    /// </para>
    /// <para>官方契约：<b>GET 且无请求体</b>；错误码 <c>-1</c>/<c>40001</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/get_current_selfmenu_info")]
    Task<MpGetCurrentSelfMenuInfoResponse> GetCurrentSelfMenuInfoAsync(
        CancellationToken cancellationToken = default);
}
