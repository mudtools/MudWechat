// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「文档」模块管理智能文档内容域公共 SDK
/// （发布智能文档 + 取消发布智能文档 + 修改发布页可查看范围 + 添加页面 + 更新页面 + 删除页面 + 获取页面结构
/// + 添加内容块 + 更新内容块 + 删除内容块 + 获取内容块列表 + 提交导出任务 + 查询导出任务结果
/// + 获取数据源 + 添加数据表 + 更新数据表 + 删除数据表）。
/// <para>
/// 官方对三类应用开放完全一致的 17 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocSmartDocService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocSmartDocService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocSmartDocService"/>。
/// </para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 管理文档内容族见 <see cref="IWechatWorkWedocDocumentService"/>；
/// 管理表格内容族见 <see cref="IWechatWorkWedocSpreadsheetService"/>；
/// 管理智能表格内容族见 <see cref="IWechatWorkWedocSmartSheetService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与服务商代开发（代开发自建应用）需具有「文档」权限。仅可作用于该应用自己创建的智能文档。</para>
/// <para>官方业务限制（全集）：发布智能文档内部为异步任务处理，通常耗时 3～8 秒，官方建议客户端超时设置为 10 秒以上；
/// <c>publish_range</c> 为 <c>4</c>（指定成员可见）时 <c>auth_list</c> 必填，
/// <c>auth_list[].type</c> 为 <c>1</c> 时须传 <c>userid</c>、为 <c>2</c> 时须传 <c>departmentid</c>；
/// 删除页面时该页面下所有子页面一并删除且不可恢复；分栏数量 <c>column_num</c> 有效范围为 [2, 4]，
/// 缺省或为 0 按 2 处理、超出范围自动收敛到边界；获取内容块列表 <c>limit</c> 最大值 200、默认值 200、<c>start</c> 最小值 0；
/// 导出内容块 <c>content_type</c> 目前仅支持 <c>1</c>（Markdown），不传 <c>page_id</c> 导出整篇文档、传入则仅导出该页面及其子 Block；
/// 添加 / 更新数据表的 <c>after_id</c> 只支持数据表类型的 <c>block_id</c>，不支持非数据表类型；
/// 删除数据表仅删除智能文档中关联的数据表内容块，不等同于删除数据表中的业务数据。</para>
/// <para>常见错误码：640008 权限不足（调用者无管理权限）、640012 <c>auth_list</c> 中 <c>userid</c> 无效（成员不在企业中）、
/// 640013 <c>auth_list</c> 中 <c>departmentid</c> 无效、640017 企业未开启 API 功能、640018 企业管理员未授予 API 编辑权限、
/// 640027 参数错误、640032 被企业管理端权限限制、640054 <c>docid</c> 不是智能文档类型。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocSmartDocService
{
    /// <summary>
    /// 发布智能文档
    /// <para>该接口用于发布智能文档，支持首次发布、再次发布并更新文档版本、发布时设置文档可见范围。</para>
    /// <para>官方业务限制：接口内部包含异步任务处理，通常耗时 3～8 秒，<b>建议将客户端超时时间设置为 10 秒以上</b>；
    /// <c>publish_range</c> 不传时默认为 <c>1</c>（企业内可见）；<c>publish_range</c> 为 <c>4</c>（指定成员可见）时 <c>auth_list</c> 必填。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="PublishSmartDocRequest"/>：docid / publish_range / auth_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发布页分享码（share_code）/ 发布页访问链接（publish_url）/ 发布版本号（version）/ 发布时间戳（publish_time）/ 发布页标题（publish_doc_title）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101616"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101633"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101650"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/publish")]
    Task<PublishSmartDocResponse> PublishSmartDocAsync(
        [Body] PublishSmartDocRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消发布智能文档
    /// <para>该接口用于取消发布智能文档（取消发布与删除文档是两个不同操作，本接口不删除智能文档）。</para>
    /// <para>官方业务限制：<c>docid</c> 必须是智能文档类型，否则返回 640054。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelPublishSmartDocRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101617"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101634"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101651"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/cancel_publish")]
    Task<WechatWorkResponse> CancelPublishSmartDocAsync(
        [Body] CancelPublishSmartDocRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改发布页可查看范围
    /// <para>该接口用于修改已发布智能文档的发布页可查看范围。</para>
    /// <para>官方业务限制：<c>publish_range</c> 取 <c>1</c> 企业内可见、<c>3</c> 企业内外可见、<c>4</c> 指定成员可见；
    /// <c>publish_range</c> 为 <c>4</c> 时 <c>auth_list</c> 必填，<c>auth_list[].type</c> 为 <c>1</c> 时须传 <c>userid</c>、
    /// 为 <c>2</c> 时须传 <c>departmentid</c>；企业须已开启 API 功能且管理员已授予 API 编辑权限。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartDocPublishSettingRequest"/>：docid / publish_range / auth_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101618"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101635"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101652"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/publish_setting")]
    Task<WechatWorkResponse> UpdateSmartDocPublishSettingAsync(
        [Body] UpdateSmartDocPublishSettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加页面
    /// <para>该接口用于向指定智能文档中添加页面（可创建第一层页面，也可通过 <c>info.parent_id</c> 创建子页面）。</para>
    /// <para>官方业务限制：<c>info.after_id</c> 为空表示插入到最前；<c>info.layout_mode</c> 取值见官方 <c>PageLayoutMode</c>
    /// （<c>1</c> 默认布局、<c>2</c> 纸张布局、<c>3</c> 全宽布局）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartDocPageRequest"/>：docid / info）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>添加成功后返回的页面信息（info：page_id / title / parent_id / after_id / layout_mode）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101620"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101637"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101654"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/add_page")]
    Task<AddSmartDocPageResponse> AddPageAsync(
        [Body] AddSmartDocPageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新页面
    /// <para>该接口用于更新智能文档中的某个页面（页面标题、页面布局、排序位置与父页面）。</para>
    /// <para>官方业务限制：<c>info.page_id</c> 必填，其余字段按需选传；
    /// <c>info.after_id</c> 为空表示把页面移动到最后；<c>info.parent_id</c> 用于将页面移动到其他父节点下。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartDocPageRequest"/>：docid / info）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新后的页面信息（info：page_id / title / layout_mode / after_id / parent_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101621"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101638"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101655"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/update_page")]
    Task<UpdateSmartDocPageResponse> UpdatePageAsync(
        [Body] UpdateSmartDocPageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除页面
    /// <para>该接口用于删除智能文档中的指定页面（<b>不等同于删除整个智能文档</b>）。</para>
    /// <para><b>危险操作约束</b>：删除页面时该页面下的<b>所有子页面也会一并删除</b>，且操作<b>不可恢复、不可撤销</b>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartDocPageRequest"/>：docid / page_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101622"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101639"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101656"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/delete_page")]
    Task<WechatWorkResponse> DeletePageAsync(
        [Body] DeleteSmartDocPageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取页面结构
    /// <para>该接口用于获取指定智能文档中的页面层级结构信息。</para>
    /// <para>官方业务限制：返回结果为<b>页面对象的扁平列表</b>，不直接返回嵌套树；
    /// <c>parent_id</c> 为空字符串表示根页面，调用方须按 <c>parent_id</c> 自行构建树形结构。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartDocPageHierarchyRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>智能文档中的页面层级列表（pages：page_id / parent_id / title）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101619"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101636"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101653"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/get_page_hierarchy")]
    Task<GetSmartDocPageHierarchyResponse> GetPageHierarchyAsync(
        [Body] GetSmartDocPageHierarchyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加内容块
    /// <para>该接口用于在智能文档的某个页面里添加一个或多个 Block（支持批量）。</para>
    /// <para>官方业务限制：<c>blocks[].after_id</c> 为空时插入到页面内容的最前面；<c>blocks[].parent_id</c> 指定父节点；
    /// <c>blocks[].props</c> 按 <c>blocks[].type</c> 取对应属性对象；
    /// 分栏属性 <c>column_num</c> 有效范围为 [2, 4]，小于 2 按 2 处理、大于 4 按 4 处理、缺省或为 0 按 2 处理；
    /// 官方未给出 <c>blocks</c> 数组最大数量与请求体大小限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartDocBlocksRequest"/>：docid / page_id / blocks）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>添加成功后返回的 Block 列表（blocks：id / type / title / parent_id / after_id / children / props）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101623"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101640"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101657"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/add_blocks")]
    Task<AddSmartDocBlocksResponse> AddBlocksAsync(
        [Body] AddSmartDocBlocksRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新内容块
    /// <para>该接口用于批量更新指定页面中已有的 Block（仅可更新已存在的 Block）。</para>
    /// <para>官方业务限制：<c>blocks[].id</c> 为必填定位字段；支持单个请求传入多个 Block 进行批量更新；
    /// <c>after_id</c> 为空时按官方说明表示插入到最前；官方未说明批量更新是否为原子操作，
    /// 亦未给出 <c>blocks</c> 数组最大长度、请求体大小与调用频率限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartDocBlocksRequest"/>：docid / page_id / blocks）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新成功后返回的 Block 列表（blocks：id / type / title / parent_id / children / props）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101624"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101641"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101658"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/update_blocks")]
    Task<UpdateSmartDocBlocksResponse> UpdateBlocksAsync(
        [Body] UpdateSmartDocBlocksRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除内容块
    /// <para>该接口用于批量删除指定智能文档页面中的 Block（仅能删除请求中指定 <c>docid</c> 下、指定 <c>page_id</c> 内的内容块）。</para>
    /// <para>官方业务限制：官方未明确单次删除数量上限，亦未说明不存在 / 已删除 / 不属于该页面的 Block ID 的返回形态。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartDocBlocksRequest"/>：docid / page_id / ids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101625"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101642"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101659"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/delete_blocks")]
    Task<WechatWorkResponse> DeleteBlocksAsync(
        [Body] DeleteSmartDocBlocksRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取内容块列表
    /// <para>该接口用于获取指定文档、指定页面中的 Block 信息（支持按 ID 精确获取与分批获取两种形态）。</para>
    /// <para>官方业务限制：<c>start</c> 分批起始点最小值为 <c>0</c>；<c>limit</c> 分批大小最大值为 <c>200</c>、默认值为 <c>200</c>；
    /// <c>has_more</c> <b>官方类型为字符串</b>（<c>"true"</c> / <c>"false"</c>），须以 <c>next_start</c> 作为下一次请求的 <c>start</c> 继续分页。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartDocBlockListRequest"/>：docid / page_id / ids / start / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>Block 数据（blocks）/ 是否还有更多数据（has_more，字符串）/ 下一批数据的起始点（next_start）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101626"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101643"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101660"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/get_block_list")]
    Task<GetSmartDocBlockListResponse> GetBlockListAsync(
        [Body] GetSmartDocBlockListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交导出任务
    /// <para>该接口用于提交一个<b>异步任务</b>以获取指定智能文档的内容，仅返回任务 ID，
    /// 实际文档内容须通过<a href="https://developer.work.weixin.qq.com/document/path/101627">查询任务结果</a>接口轮询获取。</para>
    /// <para>官方业务限制：<c>content_type</c> 目前<b>仅支持 <c>1</c>（Markdown 格式）</b>；
    /// 不传 <c>page_id</c> 导出整个智能文档，传入 <c>page_id</c> 则仅导出该 Page 及其所有子 Block 的内容；
    /// 官方未说明任务队列、导出文件大小、导出频率与超时时间等限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ExportSmartDocTaskRequest"/>：docid / content_type / page_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务 ID（task_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101627"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101644"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101661"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/export_task")]
    Task<ExportSmartDocTaskResponse> ExportSmartDocTaskAsync(
        [Body] ExportSmartDocTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询导出任务结果
    /// <para>该接口用于通过任务 ID 查询由「提交导出任务」创建的异步任务状态及结果，
    /// 任务完成时返回智能文档内容（当前返回格式为 Markdown）。</para>
    /// <para>官方业务限制：<c>task_done</c> 为 <c>false</c> 时表示任务仍在处理中、需继续轮询；
    /// 官方未定义轮询间隔、轮询超时时间、最大重试次数，亦未提供任务失败状态的专用返回字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartDocExportResultRequest"/>：task_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务是否已完成（task_done）/ 任务完成时返回的 Markdown 内容（content）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101627"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101644"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101661"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/get_export_result")]
    Task<GetSmartDocExportResultResponse> GetSmartDocExportResultAsync(
        [Body] GetSmartDocExportResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据源
    /// <para>该接口用于获取智能文档所绑定的数据表 docid：已绑定时返回已有数据表的 docid，
    /// 未绑定时官方会<b>自动创建数据表</b>并返回新创建的 docid。</para>
    /// <para>官方业务限制：仅返回数据源信息，不返回字段、视图或记录的具体内容；
    /// 官方未提供本端点的独立请求频率限制，亦未说明超时时间与幂等机制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartDocDataSourceRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>智能文档所绑定的数据表 docid（ss_docid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101628"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101645"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101662"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/get_smartsheet_info")]
    Task<GetSmartDocDataSourceResponse> GetSmartDocDataSourceAsync(
        [Body] GetSmartDocDataSourceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加数据表
    /// <para>该接口用于在智能文档的某个位置添加一个数据表内容块。</para>
    /// <para>官方业务限制：<c>info.after_id</c> 必须是<b>数据表类型</b>的 <c>block_id</c>，
    /// <b>不支持传入非数据表类型的 <c>block_id</c></b>；官方未说明超时、频率与标题长度限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartDocDataTableRequest"/>：docid / info）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>添加成功后返回的数据表信息（info：block_id / title / sheet_id / ss_docid / after_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101629"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101646"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101663"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/add_smartsheet")]
    Task<AddSmartDocDataTableResponse> AddDataTableAsync(
        [Body] AddSmartDocDataTableRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新数据表
    /// <para>该接口用于更新智能文档中已存在的数据表（数据表标题与排序位置）。</para>
    /// <para>官方业务限制：<c>info.block_id</c> 必填；<c>info.after_id</c> 留空表示把数据表移动到最后，
    /// 且<b>不支持填写非数据表类型的 <c>block_id</c></b>；<c>title</c> 与 <c>after_id</c> 均为可选字段；
    /// 本端点更新的是智能文档中的数据表关联信息，不直接更新数据表内的字段或记录。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartDocDataTableRequest"/>：docid / info）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新成功后返回的数据表信息（info：block_id / title / sheet_id / ss_docid / after_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101631"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101648"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101665"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/update_smartsheet")]
    Task<UpdateSmartDocDataTableResponse> UpdateDataTableAsync(
        [Body] UpdateSmartDocDataTableRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除数据表
    /// <para>该接口用于删除智能文档中关联的数据表内容块。</para>
    /// <para><b>危险操作约束</b>：本端点<b>仅删除智能文档中关联的数据表内容块</b>，
    /// 不等同于删除数据表中的业务数据或其他独立资源；且删除操作<b>不可撤销</b>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartDocDataTableRequest"/>：docid / block_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101630"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101647"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101664"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartdoc/delete_smartsheet")]
    Task<WechatWorkResponse> DeleteDataTableAsync(
        [Body] DeleteSmartDocDataTableRequest request,
        CancellationToken cancellationToken = default);
}
