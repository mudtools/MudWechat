// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「微信客服」模块客服账号管理域公共 SDK
/// （客服账号添加 / 列表 / 删除 / 修改 + 获取客服账号链接）。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalKfAccountService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyKfAccountService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderKfAccountService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「微信客服-可调用接口的应用」中（账号管理族端点还须在管理后台
/// 「通过API管理会话消息」-「企业内部开发」对应应用的「可管理的客服账号」处配置至少一个客服账号）；
/// 第三方 / 代开发应用须具有「微信客服-&gt;管理账号、分配会话和收发消息」权限（账号管理族端点）
/// 或「微信客服-&gt;获取基础信息」权限（查询类端点）。
/// </para>
/// <para>
/// 官方约束：只能通过 API 管理企业指定的客服账号，企业须在管理后台
/// 「微信客服-通过API管理微信客服账号」处设置对应账号允许 API 管理；
/// 一家企业最多可添加 5000 个客服账号；操作的客服账号对应的接待人员应在应用的可见范围内；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfAccountService
{
    /// <summary>
    /// 添加客服账号
    /// <para>创建客服账号并设置客服名称与头像；通过接口创建的客服账号，调用方应用将自动拥有该客服账号的管理权限。</para>
    /// <para>一家企业最多可添加 5000 个客服账号。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="AddKfAccountRequest"/>：name / media_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新创建的客服账号 ID（open_kfid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94662"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94662"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96404"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/account/add")]
    Task<AddKfAccountResponse> AddAccountAsync(
        [Body] AddKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客服账号列表
    /// <para>分页获取企业客服账号列表（含客服 ID、名称与头像）。
    /// 分页形态为 offset + limit：当返回的账号数量小于指定的 limit 时，表示已无更多数据，应终止获取。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="GetKfAccountListRequest"/>：offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客服账号信息列表（account_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94661"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94661"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96415"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/account/list")]
    Task<GetKfAccountListResponse> GetAccountListAsync(
        [Body] GetKfAccountListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客服账号
    /// <para>删除指定的客服账号；只能通过 API 管理企业指定的客服账号，
    /// 操作的客服账号对应的接待人员应在应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteKfAccountRequest"/>：open_kfid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94663"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94663"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96405"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/account/del")]
    Task<WechatWorkResponse> DeleteAccountAsync(
        [Body] DeleteKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改客服账号
    /// <para>修改已有客服账号的名称与头像，两个字段均可选填，不需要修改的可不填；
    /// 只能通过 API 管理企业指定的客服账号。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="UpdateKfAccountRequest"/>：open_kfid / name / media_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94664"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94664"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96406"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/account/update")]
    Task<WechatWorkResponse> UpdateAccountAsync(
        [Body] UpdateKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客服账号链接
    /// <para>获取客服账号的咨询链接，可嵌入 H5 页面供用户点击发起咨询，也可据此自行生成二维码；
    /// 返回的客服链接不能修改或复制参数到其他链接使用，否则进入会话事件参数校验不通过，导致无法回调。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetKfAccountContactWayRequest"/>：open_kfid / scene）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客服链接（url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94665"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94665"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96416"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/add_contact_way")]
    Task<GetKfAccountContactWayResponse> GetAccountContactWayAsync(
        [Body] GetKfAccountContactWayRequest request,
        CancellationToken cancellationToken = default);
}
