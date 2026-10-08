// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「文档」模块管理收集表域公共 SDK
/// （创建收集表 + 编辑收集表 + 获取收集表信息 + 收集表的统计信息查询 + 读取收集表答案）。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocFormService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocFormService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocFormService"/>。
/// </para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 管理文档内容族见 <see cref="IWechatWorkWedocDocumentService"/>；
/// 管理表格内容族见 <see cref="IWechatWorkWedocSpreadsheetService"/>；
/// 管理智能表格内容族见 <see cref="IWechatWorkWedocSmartSheetService"/>；
/// 管理智能文档内容族见 <see cref="IWechatWorkWedocSmartDocService"/>；
/// 设置文档权限族见 <see cref="IWechatWorkWedocDocPermissionService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与服务商代开发（代开发自建应用）需具有「文档」权限。仅可操作该应用自己创建的收集表。</para>
/// <para>官方业务限制（全集）：问题数组不超过 200 个，问题 id（<c>question_id</c>）从 1 开始、
/// 家校范围收集表从 2 开始；文本题 <c>char_len</c> 必须大于 0 且最大 4000；图片/文件题数量限制取值范围 [1, 9]（默认 9）、
/// 单文件大小上限 3000MB；多选题 <c>number</c> 不能超过选项 <c>option_item</c> 个数且 <c>type</c> 为 1/2/3 时必须指定并大于 0；
/// 时长题 <c>day_range</c> 取值范围 [1, 24]（默认 24）；定时重复与定时结束互斥、若都填优先定时重复，
/// <c>week_flag</c>/<c>skip_holiday</c>/<c>day_of_month</c> 分别仅可在对应 <c>repeat_type</c> 下填写，
/// 开启定时重复时 <c>fill_in_range</c> 必填；统计信息查询 <c>limit</c> 最大 10000、
/// <c>start_time</c>/<c>end_time</c> 仅在拉取已提交列表（req_type = 2）时必填；读取收集表答案 <c>answer_ids</c> 批次大小最大 100。</para>
/// <para>常见错误口径：匿名填写时 <c>userid</c>、<c>user_name</c>、<c>tmp_external_userid</c> 均不返回；
/// 外部用户临时 id（<c>tmp_external_userid</c>）同一用户在不同收集表中不一致，须先经转换接口转为 external_userid 才能识别身份；
/// 答案状态（<c>answer_status</c>）为 3 表示答案已被统计者移除或删除。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocFormService
{
    /// <summary>
    /// 创建收集表
    /// <para>该接口用于创建收集表（支持配置问题列表、填写权限、定时重复、匿名填写等设置）。</para>
    /// <para>官方业务限制：问题数组不超过 200 个，<c>question_id</c> 从 1 开始（家校范围收集表从 2 开始）；
    /// 文本题 <c>char_len</c> 必须大于 0 且最大 4000；图片/文件题数量限制取值范围 [1, 9]（默认 9）、单文件大小上限 3000MB；
    /// 多选题 <c>number</c> 不能超过选项 <c>option_item</c> 个数，<c>type</c> 为 1/2/3 时必须指定且大于 0；
    /// 时长题 <c>day_range</c> 取值范围 [1, 24]（默认 24）；定时重复与定时结束互斥、若都填优先定时重复；
    /// <c>week_flag</c>/<c>skip_holiday</c>/<c>day_of_month</c> 分别仅可在对应 <c>repeat_type</c> 下填写；
    /// 定时重复开启时 <c>fill_in_range</c> 必填。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateWedocFormRequest"/>：spaceid / fatherid / form_info）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>收集表 id（formid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97462"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97472"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/create_form")]
    Task<CreateWedocFormResponse> CreateFormAsync(
        [Body] CreateWedocFormRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑收集表
    /// <para>该接口用于编辑收集表：<c>oper</c> 为 <c>1</c> 时全量修改问题（标题/描述/表头/问题列表），为 <c>2</c> 时全量修改设置。</para>
    /// <para>官方业务限制：若收集表当前为家校范围，<c>fill_out_auth</c> 无法修改且问题 id 从 2 开始；
    /// <c>timed_finish</c> 与定时重复互斥、若都填优先定时重复；两种操作各自对应不同字段组，须按官方契约选传。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ModifyWedocFormRequest"/>：oper / formid / form_info）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97816"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97820"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/modify_form")]
    Task<WechatWorkResponse> ModifyFormAsync(
        [Body] ModifyWedocFormRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取收集表信息
    /// <para>该接口用于读取收集表的信息（问题列表与收集表设置）。</para>
    /// <para>官方业务限制：仅可操作该应用创建的收集表；
    /// 返回的 <c>repeated_id</c> 为收集表周期 id，是统计信息查询与读取收集表答案的定位字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocFormInfoRequest"/>：formid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>收集表信息（form_info）/ 收集表的周期 id 列表（repeated_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97817"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97821"/></para>
    /// <para>官方业务限制：只能操作该应用创建的文档。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/get_form_info")]
    Task<GetWedocFormInfoResponse> GetFormInfoAsync(
        [Body] GetWedocFormInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 收集表的统计信息查询
    /// <para>该接口用于查询收集表的统计信息：仅获取统计结果、获取已提交列表、获取未提交列表三种请求类型。</para>
    /// <para>官方业务限制：<c>req_type</c> 取 <c>1</c> 仅统计结果、<c>2</c> 已提交列表、<c>3</c> 未提交列表；
    /// 拉取已提交列表时 <c>start_time</c>/<c>end_time</c> 必填（其余类型不可传，筛选以整天为界）；
    /// <c>limit</c> 批次大小最大 10000，<c>cursor</c> 分页首次不传；
    /// 未提交列表仅当收集表限制了提交范围时才有结果；
    /// 匿名填写时 <c>userid</c>、<c>user_name</c>、<c>tmp_external_userid</c> 均不返回。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocFormStatisticRequest"/>：repeated_id / req_type / start_time / end_time / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已填写次数（fill_cnt）/ 已填写人数（fill_user_cnt）/ 未填写人数（unfill_user_cnt）/ 已填写人列表（submit_users）/ 未填写人列表（unfill_users）/ 是否还有更多（has_more）/ 分页游标（cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97818"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97822"/></para>
    /// <para>官方业务限制：只能操作该应用创建的文档；
    /// <c>tmp_external_userid</c> 同一用户在不同收集表中不一致，须先经转换接口转为 external_userid 才能识别外部填写人身份。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/get_form_statistic")]
    Task<GetWedocFormStatisticResponse> GetFormStatisticAsync(
        [Body] GetWedocFormStatisticRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取收集表答案
    /// <para>该接口用于按答案 id 批量读取收集表的答案明细（各题型的回答内容）。</para>
    /// <para>官方业务限制：<c>answer_ids</c> 批次大小最大 100；
    /// 匿名填写时不返回 <c>tmp_external_userid</c> 与 <c>userid</c>；
    /// <c>answer_status</c> 为 3 表示答案已被统计者移除或删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocFormAnswerRequest"/>：repeated_id / answer_ids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>答案明细（answer：answer_list，含 answer_id / user_name / ctime / mtime / reply / answer_status / tmp_external_userid / userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97819"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97823"/></para>
    /// <para>官方业务限制：只能操作该应用创建的文档；
    /// <c>repeated_id</c> 来源于「获取收集表信息」接口的返回。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/get_form_answer")]
    Task<GetWedocFormAnswerResponse> GetFormAnswerAsync(
        [Body] GetWedocFormAnswerRequest request,
        CancellationToken cancellationToken = default);
}
