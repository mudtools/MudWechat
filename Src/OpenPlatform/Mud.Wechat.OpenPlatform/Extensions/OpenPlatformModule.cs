// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Extensions;

/// <summary>
/// 微信开放平台 API 模块枚举（对齐 <c>MpModule</c> / <c>PayModule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>线内枚举，不动企微线 <c>WechatModule</c></b>（那被企微专属域占用）：开放平台是独立产品线，
/// 模块枚举与注册器同文件同批维护 —— 新增枚举值必须同步在 <see cref="OpenPlatformServiceBuilder"/>
/// 的注册表登记对应 <c>Add{组名}WebApiHttpClient()</c>，缺项即该域客户端静默不注册。
/// </para>
/// </remarks>
public enum OpenPlatformModule
{
    /// <summary>第三方平台管理域（<c>/cgi-bin/component/*</c> 管理面 + 免令牌推票引导；注册组 <c>Component</c>）。</summary>
    Component,

    /// <summary>开放账号管理域（<c>/cgi-bin/open/*</c>；注册组 <c>OpenAccount</c>）。</summary>
    OpenAccount,

    /// <summary>授权账号管理域（<c>/cgi-bin/account/*</c>；注册组 <c>Account</c>）。</summary>
    Account,

    /// <summary>代公众号网页授权域（<c>/sns/oauth2/component/*</c>；注册组 <c>Sns</c>）。</summary>
    Sns,

    /// <summary>小程序类目管理域（<c>/cgi-bin/wxopen/*</c> 类目/扫码购/基础库版本/公众号关联小程序；注册组 <c>Category</c>）。</summary>
    Category,

    /// <summary>
    /// 批量代云开发域（<c>/componenttcb/*</c> 全 50 端点：环境 7 + 数据库 9 + 存储 4 + 云函数 7 + 云托管 19 + 静态网站 4；注册组 <c>ComponentTcb</c>）。
    /// </summary>
    /// <remarks>
    /// 六支接口共用一个注册组 ⇒ 一次 <c>AddComponentTcbWebApiHttpClient()</c> 覆盖全部接口；
    /// 令牌恒为 <c>component_access_token</c>（Query 参数名仍是 <c>access_token</c>），
    /// 与小程序线 <c>/tcb/*</c>（<c>MpTokenTypes.AccessToken</c>）为两套路由、两套令牌，交叉声明由守卫 CBZ2 拒绝。
    /// </remarks>
    ComponentTcb,

    /// <summary>
    /// 半屏小程序 / 嵌入式小程序管理域（<c>/wxaapi/wxaembedded/*</c> 全 6 端点：添加 / 删除 / 取消授权 / 设置授权方式 4 POST + 调用列表 / 授权列表 2 GET；注册组 <c>Embedded</c>）。
    /// </summary>
    /// <remarks>
    /// 令牌恒为 <c>authorizer_access_token</c>（Query 参数名仍是 <c>access_token</c>）⇒ 属开放平台线（第三方代授权方小程序调用）。
    /// 该域页面只在 <c>doc/oplatform/…/miniprogram-management/embedded-management/</c> 树在册，小程序线不得声明之
    /// （留档 MP-D-BUCKET-RULING-01 + 小程序守卫 NC2 锁定；令牌值与小程序线 <c>MpTokenTypes.AccessToken</c> 不同源）。
    /// </remarks>
    Embedded,

    /// <summary>
    /// 小程序微信认证 / 安全认证域（<c>/wxa/sec/*</c> 全 5 端点：身份选项树 1 GET + 提审 / 重新提审 / 进度查询 3 POST + 补充材料上传 1 multipart POST；注册组 <c>Auth</c>）。
    /// </summary>
    /// <remarks>
    /// 令牌恒为 <c>authorizer_access_token</c>（Query 参数名仍是 <c>access_token</c>）⇒ 属开放平台线（第三方代授权方小程序发起认证）。
    /// 该域页面只在 <c>doc/oplatform/…/miniprogram-management/weapp-wxverify/</c> 树在册，小程序线不得声明之
    /// （留档 MP-D-BUCKET-RULING-01 + 小程序守卫 NC2 锁定）。<c>/wxa/sec/authidentitytree</c> 官方动词为 GET。
    /// </remarks>
    Auth,

    /// <summary>
    /// 流量主代运营（AMS）管理面域（<c>/wxa/operationams</c> 单路由 <c>action</c> 分派全 19 端点；注册组 <c>Ams</c>）。
    /// </summary>
    /// <remarks>
    /// 令牌恒为 <c>authorizer_access_token</c>（Query 参数名 <c>access_token</c>）⇒ 第三方平台代授权方小程序管理广告位。
    /// 应答信封为 <c>{ret, err_msg}</c>（非本线其它域的 <c>{errcode, errmsg}</c>），判错基底另立 <c>OpenPlatformAmsResponse</c>。
    /// </remarks>
    Ams,

    /// <summary>
    /// 流量主代运营（AMS）分账面域（<c>/wxa/getdefaultamsinfo</c> + <c>/wxa/setdefaultamsinfo</c> 全 3 端点；注册组 <c>AmsShare</c>）。
    /// </summary>
    /// <remarks>
    /// 令牌恒为 <c>component_access_token</c>（Query 参数名 <c>access_token</c>）⇒ 服务商平台自身面，与 <see cref="Ams"/>（授权方令牌）令牌归属不同，
    /// 故拆独立注册组；两支同路由多端点各以 <c>action</c> query 分派。
    /// </remarks>
    AmsShare,
}
