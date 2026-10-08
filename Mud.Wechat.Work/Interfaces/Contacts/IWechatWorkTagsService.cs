// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contacts.Tags;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信通讯录「标签管理」域公共 SDK（创建、更新名字、删除、获取成员、增加成员、删除成员、获取标签列表）。
/// <para>
/// 与成员 / 部门域不同，官方对自建应用、第三方应用、服务商代开发三类应用开放了完全一致的 7 个标签端点，
/// 因此全部端点声明于本公共父接口；三个应用类型子接口
/// （<see cref="IWechatWorkInternalTagsService"/> / <see cref="IWechatWorkThirdPartyTagsService"/> /
/// <see cref="IWechatWorkProviderTagsService"/>）均为空标记，仅作为类型化契约入口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkUsersService"/>：三类应用消费的令牌路由键均为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：标签写入端点要求「调用的应用必须是指定标签的创建者」，增删成员还要求成员属于应用可见范围；
/// 获取标签列表时自建应用 / 通讯录同步助手可获取所有标签，第三方与代开发应用仅可获取自己创建的标签。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkTagsService
{
    /// <summary>
    /// 创建标签
    /// <para>向企业通讯录写入新标签。tagname 必填（≤ 32 个字，全局不重名）；未指定 tagid 时由官方以目前最大 ID 自增。</para>
    /// <para>创建的标签属于该应用，只有该应用的 secret 才可以增删成员；标签总数不能超过 3000 个。</para>
    /// </summary>
    /// <param name="request">标签请求体（<see cref="CreateTagRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（含新建标签的 tagid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90210"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90346"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96278"/></para>
    /// </remarks>
    [Post("/cgi-bin/tag/create")]
    Task<CreateTagResponse> CreateTagAsync(
        [Body] CreateTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新标签名字
    /// <para>更新通讯录中的既有标签名称。调用的应用必须是指定标签的创建者；标签名全局不可重名。</para>
    /// </summary>
    /// <param name="request">标签请求体（<see cref="UpdateTagNameRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90211"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90347"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96279"/></para>
    /// </remarks>
    [Post("/cgi-bin/tag/update")]
    Task<WechatWorkResponse> UpdateTagNameAsync(
        [Body] UpdateTagNameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除标签
    /// <para>从企业通讯录中删除标签。调用的应用必须是指定标签的创建者。</para>
    /// </summary>
    /// <param name="tagId">标签 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90212"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90348"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96280"/></para>
    /// </remarks>
    [Get("/cgi-bin/tag/delete")]
    Task<WechatWorkResponse> DeleteTagAsync(
        [Query("tagid")] int tagId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取标签成员
    /// <para>获取标签内的成员（企业成员与部门）列表；无调用限制，但返回列表仅包含应用可见范围内的成员。</para>
    /// </summary>
    /// <param name="tagId">标签 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标签成员列表（tagname / userlist / partylist；成员 name 字段按官方停返口径可能为空）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90213"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90349"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96281"/></para>
    /// </remarks>
    [Get("/cgi-bin/tag/get")]
    Task<GetTagMembersResponse> GetTagMembersAsync(
        [Query("tagid")] int tagId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加标签成员
    /// <para>向标签追加成员。调用的应用必须是指定标签的创建者，且成员属于应用的可见范围。</para>
    /// <para>userlist 与 partylist 不能同时为空；每个标签下部门数和人员数总和不能超过 3 万个；单次请求 userlist ≤ 1000、partylist ≤ 100。</para>
    /// </summary>
    /// <param name="request">标签成员请求体（<see cref="AddTagMembersRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>三态结果（<see cref="ChangeTagMembersResponse"/>：全部合法 / 部分非法附 invalidlist、invalidparty / 全部非法报错）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90214"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90350"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96282"/></para>
    /// </remarks>
    [Post("/cgi-bin/tag/addtagusers")]
    Task<ChangeTagMembersResponse> AddTagMembersAsync(
        [Body] AddTagMembersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除标签成员
    /// <para>从标签移除成员。调用的应用必须是指定标签的创建者，且成员属于应用的可见范围。</para>
    /// <para>userlist 与 partylist 不能同时为空；单次请求 userlist ≤ 1000、partylist ≤ 100。</para>
    /// </summary>
    /// <param name="request">标签成员请求体（<see cref="RemoveTagMembersRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>三态结果（<see cref="ChangeTagMembersResponse"/>：全部合法 / 部分非法附 invalidlist、invalidparty / 全部非法报错）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90215"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90351"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96283"/></para>
    /// </remarks>
    [Post("/cgi-bin/tag/deltagusers")]
    Task<ChangeTagMembersResponse> RemoveTagMembersAsync(
        [Body] RemoveTagMembersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取标签列表
    /// <para>获取企业通讯录中的标签列表。自建应用 / 通讯录同步助手可获取所有标签；第三方与代开发应用仅可获取自己创建的标签。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标签列表（taglist）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90216"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90352"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96284"/></para>
    /// </remarks>
    [Get("/cgi-bin/tag/list")]
    Task<GetTagListResponse> GetTagListAsync(
        CancellationToken cancellationToken = default);
}
