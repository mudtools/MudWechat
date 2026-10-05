// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「文档」模块管理智能表格内容域公共 SDK
/// （添加子表 + 删除子表 + 更新子表 + 查询子表 + 添加视图 + 删除视图 + 更新视图 + 查询视图 + 添加字段 + 删除字段 + 更新字段 + 查询字段 + 添加记录 + 删除记录 + 更新记录 + 查询记录 + 添加编组 + 删除编组 + 更新编组 + 获取编组）。
/// <para>
/// 官方对三类应用开放完全一致的 20 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocSmartSheetService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocSmartSheetService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocSmartSheetService"/>。
/// </para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 管理文档内容族见 <see cref="IWechatWorkWedocDocumentService"/>；
/// 管理表格内容族见 <see cref="IWechatWorkWedocSpreadsheetService"/>；
/// 管理智能文档内容族见 <see cref="IWechatWorkWedocSmartDocService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与服务商代开发（代开发自建应用）需具有「文档」权限。仅可作用于该应用自己创建的文档/智能表格。</para>
/// <para>官方业务限制（全集）：添加视图单表最多 200 个视图；添加字段单表最多 150 个字段；
/// 添加编组单表最多 150 个编组、每个编组最多 150 个字段且字段只能同时存在于一个编组；
/// 添加记录单表最多 100000 行记录、15000000 个单元格（单次添加建议在 500 行内）；
/// 删除记录单次删除建议在 500 行内；更新记录单次更新建议在 500 行内；
/// 查询视图 / 查询字段 / 查询记录分页 <c>limit</c> 最大值 1000；
/// 创建时间、最后编辑时间、创建人、最后编辑人四种类型的字段不可通过记录接口写入；
/// 查询记录的 <c>filter_spec</c> 不支持与 <c>sort</c> 一起使用；链接类型数组为预留能力（目前只支持展示一个链接）。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocSmartSheetService
{
    /// <summary>
    /// 添加子表
    /// <para>该接口用于在在线表格的某个位置添加一个智能表（Smartsheet 子表）；该智能表不存在视图、记录和字段，需再调用添加视图 / 添加字段 / 添加记录接口补齐内容。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartSheetSheetRequest"/>：docid / properties）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建子表的智能表属性（properties：sheet_id / title / index）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99896"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100196"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100214"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/add_sheet")]
    Task<AddSmartSheetSheetResponse> AddSheetAsync(
        [Body] AddSmartSheetSheetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除子表
    /// <para>该接口用于删除在线表格中的某个智能表（Smartsheet 子表）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartSheetSheetRequest"/>：docid / sheet_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99899"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100197"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100215"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/delete_sheet")]
    Task<WechatWorkResponse> DeleteSheetAsync(
        [Body] DeleteSmartSheetSheetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新子表
    /// <para>该接口用于更新在线表格中某个智能表（Smartsheet 子表）的子表标题。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartSheetSheetRequest"/>：docid / properties）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99898"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100198"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100216"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/update_sheet")]
    Task<WechatWorkResponse> UpdateSheetAsync(
        [Body] UpdateSmartSheetSheetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询子表
    /// <para>该接口用于查询一篇在线表格中全部（或指定）的智能表信息。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartSheetSheetRequest"/>：docid / sheet_id / need_all_type_sheet）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>智能表信息列表（sheet_list：sheet_id / title / is_visible / type）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101154"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101182"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101164"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/get_sheet")]
    Task<GetSmartSheetSheetResponse> GetSheetAsync(
        [Body] GetSmartSheetSheetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加视图
    /// <para>该接口用于在 Smartsheet 的某个子表里添加一个新视图。</para>
    /// <para>官方业务限制：单表最多允许有 200 个视图；添加甘特视图（<c>view_type</c> 为 <c>VIEW_TYPE_GANTT</c>）时 <c>property_ganttobect</c> 必填，添加日历视图（<c>view_type</c> 为 <c>VIEW_TYPE_CALENDAR</c>）时 <c>property_calendar</c> 必填。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartSheetViewRequest"/>：docid / sheet_id / view_title / view_type / property_ganttobect / property_calendar）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>添加成功的视图（view：view_id / view_title / view_type）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99900"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100199"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100217"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/add_view")]
    Task<AddSmartSheetViewResponse> AddViewAsync(
        [Body] AddSmartSheetViewRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除视图
    /// <para>该接口用于删除 Smartsheet 某个子表中的一个或多个视图。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartSheetViewsRequest"/>：docid / sheet_id / view_ids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99901"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100200"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100218"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/delete_views")]
    Task<WechatWorkResponse> DeleteViewsAsync(
        [Body] DeleteSmartSheetViewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新视图
    /// <para>该接口用于更新 Smartsheet 中的某个视图（视图标题及排序、过滤、分组、字段显示、冻结列与填色配置）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartSheetViewRequest"/>：docid / sheet_id / view_id / view_title / property）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新成功的视图（view：view_id / view_title / view_type / property）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99902"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100201"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100219"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/update_view")]
    Task<UpdateSmartSheetViewResponse> UpdateViewAsync(
        [Body] UpdateSmartSheetViewRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询视图
    /// <para>该接口用于查询 Smartsheet 某个子表中的视图列表。</para>
    /// <para>官方业务限制：<c>limit</c> 分页大小最大值为 1000；不填写或设置为 0 时，总数大于 1000 一次性返回 1000 个视图，总数小于 1000 时返回全部视图。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartSheetViewsRequest"/>：docid / sheet_id / view_ids / offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>视图总数（total）/ 是否还有更多项（has_more）/ 下次搜索偏移量（next）/ 视图数据（views）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101155"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101183"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101165"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/get_views")]
    Task<GetSmartSheetViewsResponse> GetViewsAsync(
        [Body] GetSmartSheetViewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加字段
    /// <para>该接口用于在智能表中的某个子表里添加一列或多列新字段。</para>
    /// <para>官方业务限制：单表最多允许有 150 个字段；<b>字段属性与字段类型是匹配的，一种字段类型对应一种字段属性</b>，新增字段不传 <c>field_id</c>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartSheetFieldsRequest"/>：docid / sheet_id / fields）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建成功的字段详情（fields：field_id / field_title / field_type）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99904"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100202"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100220"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/add_fields")]
    Task<AddSmartSheetFieldsResponse> AddFieldsAsync(
        [Body] AddSmartSheetFieldsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除字段
    /// <para>该接口用于删除智能表中的某个子表里的一列或多列字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartSheetFieldsRequest"/>：docid / sheet_id / field_ids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99905"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100203"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100221"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/delete_fields")]
    Task<WechatWorkResponse> DeleteFieldsAsync(
        [Body] DeleteSmartSheetFieldsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新字段
    /// <para>该接口用于更新智能表中的某个子表里的一列或多列字段（仅可更新字段名与字段属性）。</para>
    /// <para>官方业务限制：该接口只能更新字段名、字段属性，<b>不能更新字段类型</b>；更新时 <c>field_title</c> 和 <c>property_number</c> 至少需要传一个，<c>field_title</c> 不能被更新为原值；<c>field_type</c> 必须为原属性；字段属性与字段类型是匹配的，一种字段类型对应一种字段属性。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartSheetFieldsRequest"/>：docid / sheet_id / fields）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新成功的字段详情（fields：field_id / field_title / field_type）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99906"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100204"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100222"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/update_fields")]
    Task<UpdateSmartSheetFieldsResponse> UpdateFieldsAsync(
        [Body] UpdateSmartSheetFieldsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询字段
    /// <para>该接口用于查询智能表某个子表中的字段详情。</para>
    /// <para>官方业务限制：<c>limit</c> 分页大小最大值为 1000；不填写或设置为 0 时，总数大于 1000 一次性返回 1000 个字段，总数小于 1000 时返回全部字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartSheetFieldsRequest"/>：docid / sheet_id / view_id / field_ids / field_titles / offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>字段总数（total）/ 字段详情（fields）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101157"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100223"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101166"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/get_fields")]
    Task<GetSmartSheetFieldsResponse> GetFieldsAsync(
        [Body] GetSmartSheetFieldsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加记录
    /// <para>该接口用于在智能表中的某个子表里添加一行或多行记录。</para>
    /// <para>官方业务限制：单表最多允许有 100000 行记录、15000000 个单元格；<b>单次添加建议在 500 行内</b>；<b>不能通过添加记录接口给创建时间、最后编辑时间、创建人和最后编辑人四种类型的字段添加记录</b>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartSheetRecordsRequest"/>：docid / sheet_id / key_type / records）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>添加成功的记录具体内容（records：record_id / values）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99907"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101184"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100224"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/add_records")]
    Task<AddSmartSheetRecordsResponse> AddRecordsAsync(
        [Body] AddSmartSheetRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除记录
    /// <para>该接口用于删除 Smartsheet 某个子表中的一行或多行记录。</para>
    /// <para>官方业务限制：单次删除建议在 500 行内。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartSheetRecordsRequest"/>：docid / sheet_id / record_ids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99908"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100206"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100225"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/delete_records")]
    Task<WechatWorkResponse> DeleteRecordsAsync(
        [Body] DeleteSmartSheetRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新记录
    /// <para>该接口用于更新 Smartsheet 中的某个子表里的一行或多行记录。</para>
    /// <para>官方业务限制：单次更新建议在 500 行内；<b>不能通过更新记录接口给创建时间、最后编辑时间、创建人和最后编辑人四种类型的字段更新记录</b>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartSheetRecordsRequest"/>：docid / sheet_id / key_type / records）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新成功的记录具体内容（records：record_id / values）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99909"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100207"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100226"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/update_records")]
    Task<UpdateSmartSheetRecordsResponse> UpdateRecordsAsync(
        [Body] UpdateSmartSheetRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询记录
    /// <para>该接口用于查询智能表某个子表中的记录（含分页、排序与过滤）。</para>
    /// <para>官方业务限制：<c>limit</c> 分页大小最大值为 1000；不填写或设置为 0 时，总数大于 1000 一次性返回 1000 行记录，总数小于 1000 时返回全部记录；<b><c>filter_spec</c> 不支持和 <c>sort</c> 一起使用</b>；链接类型的数组为预留能力，<b>目前只支持展示一个链接，建议只传入一个链接</b>；地理位置来源目前只支持腾讯地图。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartSheetRecordsRequest"/>：docid / sheet_id / view_id / record_ids / key_type / field_titles / field_ids / sort / offset / limit / ver / filter_spec）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>符合筛选条件的记录总数（total）/ 是否还有更多项（has_more）/ 下次搜索偏移量（next）/ 记录数据（records）/ 版本号（ver）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101158"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101185"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101167"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/get_records")]
    Task<GetSmartSheetRecordsResponse> GetRecordsAsync(
        [Body] GetSmartSheetRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加编组
    /// <para>该接口用于在智能表中的某个子表里添加编组。</para>
    /// <para>官方业务限制：单表最多允许有 150 个编组；每个编组最多允许有 150 个字段；<b>字段只能同时存在于一个编组</b>；编组名称不能和已有名称重复。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddSmartSheetFieldGroupRequest"/>：docid / sheet_id / name / children）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编组信息（field_group：field_group_id / name / children）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101100"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101178"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101174"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/add_field_group")]
    Task<AddSmartSheetFieldGroupResponse> AddFieldGroupAsync(
        [Body] AddSmartSheetFieldGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除编组
    /// <para>该接口用于删除智能表中某个子表的一个或多个编组。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteSmartSheetFieldGroupsRequest"/>：docid / sheet_id / field_group_ids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101102"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101179"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101175"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/delete_field_groups")]
    Task<WechatWorkResponse> DeleteFieldGroupsAsync(
        [Body] DeleteSmartSheetFieldGroupsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新编组
    /// <para>该接口用于在智能表中的某个子表里更新已有编组。</para>
    /// <para>官方业务限制：每个编组最多允许有 150 个字段；<b>字段只能同时存在于一个编组</b>；编组名称不能和已有名称重复。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateSmartSheetFieldGroupRequest"/>：docid / sheet_id / field_group_id / name / children）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编组信息（field_group：field_group_id / name / children）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101101"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101180"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101176"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/update_field_group")]
    Task<UpdateSmartSheetFieldGroupResponse> UpdateFieldGroupAsync(
        [Body] UpdateSmartSheetFieldGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取编组
    /// <para>该接口用于获取智能表某个子表中的编组列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetSmartSheetFieldGroupsRequest"/>：docid / sheet_id / offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编组数量（total）/ 是否还有更多数据（has_more）/ 下一偏移位置（next）/ 编组列表（field_groups）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101103"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101181"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101177"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/get_field_groups")]
    Task<GetSmartSheetFieldGroupsResponse> GetFieldGroupsAsync(
        [Body] GetSmartSheetFieldGroupsRequest request,
        CancellationToken cancellationToken = default);
}
