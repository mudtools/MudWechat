// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Agent;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「应用管理」模块<b>第三方应用 + 服务商代开发</b>公共 SDK 接口。
/// <para>
/// 承载官方<b>仅向第三方应用与服务商代开发开放</b>的 2 个端点（获取应用权限、获取应用管理员列表）：
/// 两端的官方页面只存在于「第三方应用开发」与「服务商代开发」两棵章节树下
/// （获取应用权限 99052 / 99108，获取应用管理员列表 100073 / 100072），
/// 「企业自建应用开发」树无对应页面，故本接口族端点<b>不由自建应用子接口继承</b>。
/// </para>
/// <para>
/// 继承公共父接口 <see cref="IWechatWorkAgentService"/> 的三类应用公共面 2 个端点
/// （获取指定的应用详情 / 获取 access_token 对应的应用列表）——合计 4 个端点，
/// 与官方对第三方及代开发开放的应用管理端点集一致。
/// </para>
/// <para>
/// 应用类型子接口：<see cref="IWechatWorkThirdPartyAgentService"/>（第三方）、
/// <see cref="IWechatWorkProviderAgentService"/>（服务商代开发），二者均为零差异端点空标记；
/// 自建应用见 <see cref="IWechatWorkInternalAgentService"/>，直接继承公共父接口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 两族应用均消费<b>授权企业级</b> access_token（scope = authCorpId），
/// 调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// 官方契约形态：两个端点均为 POST 且<b>请求体不含业务字段</b>（仅 access_token 走 Query），
/// 故方法不声明 <c>[Body]</c> 参数，与「获取应用共享信息」等带包体端点区分。
/// </para>
/// <para>
/// 官方权限说明（两页正文逐字一致，第三方树 99052/100073 与代开发树 99108/100072 为同一页面内容）：
/// 获取应用权限详情「支持所有的代开发应用、第三方应用以及获客助手组件调用」；
/// 获取应用管理员列表为「第三方服务商可以用此接口获取授权企业中某个第三方应用或者代开发应用的管理员列表
/// (不包括外部管理员)，以便服务商在用户进入应用主页之后根据是否管理员身份做权限的区分」。
/// 两页的「参数说明」表<b>只列 access_token</b>，无任何业务入参。
/// </para>
/// <para>
/// 官方限制：两页正文均未给出本端点的独立频率数字，故按附录「访问频率限制」的默认口径执行
/// （每企业单个 cgi/api 1 万次/分、15 万次/小时；第三方提供商每 IP 4 万次/分、120 万次/小时）。
/// 该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true,
    InheritedFrom = nameof(WechatWorkAgentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAgentThirdPartyProviderService : IWechatWorkAgentService
{
    /// <summary>
    /// 获取应用权限详情
    /// <para>官方原文：「该API用于获取企业授权的代开发应用或第三方应用或获客助手组件的权限详情。」
    /// 即返回当前授权关系下、应用需要申请的权限列表。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>权限列表（官方 app_permissions，字符串数组）与二次授权时效扩展信息（app_permissions_ext）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99052"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99108"/></para>
    /// <para>官方业务限制：企业自建应用文档树无本端点页面（不开放）；官方请求体不含业务字段，
    /// 故方法不声明请求体参数；响应权限列表字段名为 <c>app_permissions</c>（数组），
    /// 元素是「分类:子分类:权限」形态的字符串，其取值字典在该页附录「应用权限说明」中按分类给出
    /// （通讯录 / 成员敏感信息 / 组织架构 / 企业客户等，且区分「仅第三方应用返回」与「仅代开发应用返回」），
    /// 附录随官方扩充，故 SDK 不做枚举建模。</para>
    /// <para>官方字段口径：<c>app_permissions_ext</c> 为「应用权限的扩展信息，若存在应用权限为二次授权
    /// 并且有时效时，返回该字段列表」，其 <c>expire_time</c> 为秒级时间戳，
    /// 且「若权限不会过期则 expire_time 字段值为 0」。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/get_permissions")]
    Task<GetAgentPermissionsResponse> GetAgentPermissionsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取应用管理员列表
    /// <para>官方原文：「第三方服务商可以用此接口获取授权企业中某个第三方应用或者代开发应用的管理员列表
    /// (不包括外部管理员)，以便服务商在用户进入应用主页之后根据是否管理员身份做权限的区分。」</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>管理员列表（官方 admin，元素含 userid 与 auth_type，见 <see cref="AgentAdminItem"/>）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100073"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100072"/></para>
    /// <para>官方业务限制：企业自建应用文档树无本端点页面（不开放）；官方请求体不含业务字段，
    /// 故方法不声明请求体参数；响应管理员列表字段名为 <c>admin</c>（不带 <c>_list</c> 后缀），
    /// 与元素结构不同的旧服务商形态 <c>/cgi-bin/service/get_admin_list</c>（以 <c>suite_access_token</c> 调用、
    /// 元素多回传 <c>open_userid</c>）已脱离官方目录树，本仓不声明（零声明守卫 RD1 锁定）。</para>
    /// <para>官方字段口径：<c>admin</c> 为「应用的管理员列表（不包括外部管理员）。成员授权模式下，不返回管理员列表」，
    /// <c>admin.auth_type</c> 为「该管理员对应用的权限：0=发消息权限，1=管理权限」；
    /// <c>errcode</c> 为 0 才表示「正常返回，可读取 admin 的管理员列表」。</para>
    /// <para>官方提示：「旧的使用 suite_access_token 获取管理员的方式已不再维护，建议使用当前新的接口」——
    /// 本端点用授权企业级 access_token，而非 suite_access_token。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/get_admin_list")]
    Task<GetAgentAdminListResponse> GetAgentAdminListAsync(
        CancellationToken cancellationToken = default);
}
