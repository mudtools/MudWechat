// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微信客服」模块机器人管理域企业自建应用 SDK：
/// 官方仅向自建应用开放本域端点（第三方应用与服务商代开发暂不支持），
/// 知识库分组管理 4 个端点 + 知识库问答管理 4 个端点均声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 应用须配置到「微信客服-可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Kf",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkKfKnowledgeService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalKfKnowledgeService : IWechatWorkKfKnowledgeService
{
    /// <summary>
    /// 知识库添加分组
    /// <para>为知识库添加一个分组；分组名不可重复，分组总数上限 100。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="AddKfKnowledgeGroupRequest"/>：name）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新创建的分组 ID（group_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95971"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/add_group")]
    Task<AddKfKnowledgeGroupResponse> AddKnowledgeGroupAsync(
        [Body] AddKfKnowledgeGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库删除分组
    /// <para>删除知识库分组；默认分组（is_default = 1）不可删除。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteKfKnowledgeGroupRequest"/>：group_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95971"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/del_group")]
    Task<WechatWorkResponse> DeleteKnowledgeGroupAsync(
        [Body] DeleteKfKnowledgeGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库修改分组
    /// <para>修改知识库分组名称；分组名不可重复，默认分组（is_default = 1）不可修改。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="UpdateKfKnowledgeGroupRequest"/>：group_id / name）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95971"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/mod_group")]
    Task<WechatWorkResponse> UpdateKnowledgeGroupAsync(
        [Body] UpdateKfKnowledgeGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库获取分组列表
    /// <para>分页获取知识库分组列表（cursor + limit + has_more 游标分页；limit 默认 500、最大 1000）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetKfKnowledgeGroupListRequest"/>：cursor / limit / group_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分组列表（group_list）、分页游标（next_cursor）与是否还有更多数据（has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95971"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/list_group")]
    Task<GetKfKnowledgeGroupListResponse> GetKnowledgeGroupListAsync(
        [Body] GetKfKnowledgeGroupListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库添加问答
    /// <para>向指定分组添加一条问答（问题 + 相似问题 + 答案）；
    /// 不同分组的问题不能重复，单个分组问答数上限 200，答案目前仅支持 1 个。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="AddKfKnowledgeIntentRequest"/>：group_id / question / similar_questions / answers）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新创建的问答 ID（intent_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95972"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/add_intent")]
    Task<AddKfKnowledgeIntentResponse> AddKnowledgeIntentAsync(
        [Body] AddKfKnowledgeIntentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库删除问答
    /// <para>按问答 ID 删除知识库问答。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteKfKnowledgeIntentRequest"/>：intent_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95972"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/del_intent")]
    Task<WechatWorkResponse> DeleteKnowledgeIntentAsync(
        [Body] DeleteKfKnowledgeIntentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库修改问答
    /// <para>修改知识库问答；修改为<b>整体覆盖写</b>——即便只修改部分字段，也须传入完整字段内容。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="UpdateKfKnowledgeIntentRequest"/>：intent_id / question / similar_questions / answers）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95972"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/mod_intent")]
    Task<WechatWorkResponse> UpdateKnowledgeIntentAsync(
        [Body] UpdateKfKnowledgeIntentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 知识库获取问答列表
    /// <para>分页获取知识库问答列表（cursor + limit + has_more 游标分页；limit 默认 500、最大 1000）；
    /// 附件中的 media_id 与小程序封面 thumb_media_id 仅写入时有效，列表查询不返回。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetKfKnowledgeIntentListRequest"/>：cursor / limit / group_id / intent_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>问答列表（intent_list）、分页游标（next_cursor）与是否还有更多数据（has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95972"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/knowledge/list_intent")]
    Task<GetKfKnowledgeIntentListResponse> GetKnowledgeIntentListAsync(
        [Body] GetKfKnowledgeIntentListRequest request,
        CancellationToken cancellationToken = default);
}
