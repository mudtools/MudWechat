// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块会议布局和背景管理域企业自建应用 SDK（承载本域全部 15 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingLayoutService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；基础布局仅允许操作该应用创建的会议；
/// 高级布局目前仅支持 H.323/SIP 会议室终端。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingLayoutService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingLayoutService : IWechatWorkMeetingLayoutService
{
    /// <summary>
    /// 获取布局模板列表
    /// <para>获取企业下所有的布局模板列表。</para>
    /// <para>官方契约：本端点为 <b>GET</b> 请求（会议域唯一 GET 端点），无请求体，仅以 Query 注入的 access_token 鉴权。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>布局模板对象列表（layout_template_list：layout_template_id / thumbnail_url / picture_url / render_rule）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98844"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Get("/cgi-bin/meeting/layout/list_template")]
    Task<ListLayoutTemplatesResponse> ListLayoutTemplatesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加会议基础布局
    /// <para>对 API 成功预定的会议添加会议基础布局，支持多个布局的添加，每个布局支持多页模板，默认选中第一页模板作为该布局的首页进行展示。</para>
    /// <para>官方限制：一场会议最多添加 10 个布局；用户座次设置区分会前和会中两种方式——会前只允许设置邀请者成员，会中只允许设置参会成员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddMeetingLayoutRequest"/>：meetingid / layout_list / default_layout_order）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议应用的布局 ID（selected_layout_id）与新增的会议布局信息列表（layout_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98845"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/add")]
    Task<AddMeetingLayoutResponse> AddMeetingLayoutAsync(
        [Body] AddMeetingLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加会议高级布局
    /// <para>对当前会议添加高级布局，支持批量添加。</para>
    /// <para>官方限制：单个会议最多允许添加 20 个高级布局；用户座次设置需设置参会成员；高级布局目前仅支持 H.323/SIP 会议室终端。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddAdvancedLayoutRequest"/>：meetingid / layout_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新增的会议布局信息列表（layout_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98861"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/advanced_layout/add")]
    Task<AddAdvancedLayoutResponse> AddAdvancedLayoutAsync(
        [Body] AddAdvancedLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改会议基础布局
    /// <para>根据布局 ID 对设置好的会议基础布局进行修改。</para>
    /// <para>官方限制：用户座次设置区分会前和会中两种方式——会前只允许设置邀请者成员，会中只允许设置参会成员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateMeetingLayoutRequest"/>：meetingid / layout_id / page_list / enable_set_default）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98846"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/update")]
    Task<WechatWorkResponse> UpdateMeetingLayoutAsync(
        [Body] UpdateMeetingLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改会议高级布局
    /// <para>对会议中的高级布局进行修改，注意修改的是布局定义。</para>
    /// <para>官方限制：若修改的会议布局正被会议使用，新布局会自动应用到会议；若修改的会议布局正在被用户使用，
    /// 新布局不会自动应用到用户；接口仅支持全量更新，不支持部分字段单独更新；高级布局目前仅支持 H.323/SIP 会议室终端。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateAdvancedLayoutRequest"/>：meetingid / layout_id / layout_name / page_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98868"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/advanced_layout/update")]
    Task<WechatWorkResponse> UpdateAdvancedLayoutAsync(
        [Body] UpdateAdvancedLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置会议默认布局
    /// <para>对 API 成功预定的会议设置默认布局。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetDefaultMeetingLayoutRequest"/>：meetingid / selected_layout_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98847"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/set_default")]
    Task<WechatWorkResponse> SetDefaultMeetingLayoutAsync(
        [Body] SetDefaultMeetingLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置高级布局
    /// <para>将会议中的高级自定义布局应用到指定成员或者整个会议，也可以恢复指定成员或整个会议的默认布局。</para>
    /// <para>官方限制：高级布局应用到指定成员目前仅支持 H.323/SIP 会议室终端；
    /// 应用布局的优先级从高到低为：个性布局 &gt; 自定义布局 &gt; 默认布局
    /// （MRA 不支持同框模式，如果会议设置为同框模式，MRA 应用默认布局）；user_list 单次最多支持 20 个用户。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ApplyAdvancedLayoutRequest"/>：meetingid / layout_id / user_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98869"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/advanced_layout/apply")]
    Task<WechatWorkResponse> ApplyAdvancedLayoutAsync(
        [Body] ApplyAdvancedLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议布局列表
    /// <para>根据会议 ID 返回会议的基础和高级自定义布局信息列表。</para>
    /// <para>官方限制：高级布局目前仅支持 H.323/SIP 会议室终端。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListMeetingLayoutsRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议应用的布局 ID（selected_layout_id）与布局对象列表（layout_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98862"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/advanced_layout/list")]
    Task<ListMeetingLayoutsResponse> ListMeetingLayoutsAsync(
        [Body] ListMeetingLayoutsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户布局
    /// <para>根据会议 ID 和用户 ID 返回用户的布局设置信息。</para>
    /// <para>官方限制：布局优先级为用户个性布局 &gt; 会议自定义布局（高级布局、基础布局）&gt; 会议默认布局；
    /// 高级布局目前仅支持 H.323/SIP 会议室终端。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetUserLayoutRequest"/>：meetingid / tmp_openid / instance_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议应用的布局 ID（selected_layout_id）、布局名称（layout_name）、布局类型（layout_type：0 默认 / 2 自定义会议布局 / 3 个性布局）与布局单页对象列表（page_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98865"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/advanced_layout/get_user_layout")]
    Task<GetUserLayoutResponse> GetUserLayoutAsync(
        [Body] GetUserLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除布局
    /// <para>根据布局 ID 批量删除布局，可以删除基础布局和高级布局。</para>
    /// <para>官方限制：正在被应用的布局无法删除，请先设置成其他布局或恢复成默认原始布局后再行删除；
    /// 接口不做布局是否存在的校验，删除不存在的布局不会有提示；最多支持 20 个布局 ID。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchDeleteLayoutsRequest"/>：meetingid / layout_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98866"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/advanced_layout/batch_delete")]
    Task<WechatWorkResponse> BatchDeleteLayoutsAsync(
        [Body] BatchDeleteLayoutsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加会议背景
    /// <para>对成功预定的会议添加会议背景，支持多个背景图片的添加。</para>
    /// <para>官方限制：一场会议最多添加 7 个背景，且仅支持不超过 10MB 大小的 PNG 格式图片，分辨率最小为 1920x1080；
    /// 背景图片上传方式为异步上传，可以通过订阅「素材上传结果」获取上传结果通知。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddMeetingBackgroundRequest"/>：meetingid / image_list / default_image_order）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议应用的背景 ID（selected_background_id）与背景对象列表（background_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98851"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/add_background")]
    Task<AddMeetingBackgroundResponse> AddMeetingBackgroundAsync(
        [Body] AddMeetingBackgroundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置会议默认背景
    /// <para>对 API 成功预定的会议设置默认背景。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetDefaultMeetingBackgroundRequest"/>：meetingid / selected_background_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98852"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/set_default_background")]
    Task<WechatWorkResponse> SetDefaultMeetingBackgroundAsync(
        [Body] SetDefaultMeetingBackgroundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议背景列表
    /// <para>根据会议 ID 返回会议背景列表信息。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListMeetingBackgroundsRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议应用的背景 ID（selected_background_id）与背景对象列表（background_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98856"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/list_background")]
    Task<ListMeetingBackgroundsResponse> ListMeetingBackgroundsAsync(
        [Body] ListMeetingBackgroundsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会议背景
    /// <para>根据背景 ID 删除单个会议背景。</para>
    /// <para>官方限制：正在被会议应用的背景无法删除，请先设置成其他背景或恢复成会议的默认黑色背景后再行删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteMeetingBackgroundRequest"/>：meetingid / background_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98853"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/delete_background")]
    Task<WechatWorkResponse> DeleteMeetingBackgroundAsync(
        [Body] DeleteMeetingBackgroundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除会议背景
    /// <para>根据背景 ID 删除多个会议背景。</para>
    /// <para>官方限制：正在被会议应用的背景无法删除，请先设置成其他背景或恢复成会议的默认黑色背景后再行删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchDeleteMeetingBackgroundsRequest"/>：meetingid / background_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98854"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/layout/batch_delete_background")]
    Task<WechatWorkResponse> BatchDeleteMeetingBackgroundsAsync(
        [Body] BatchDeleteMeetingBackgroundsRequest request,
        CancellationToken cancellationToken = default);
}
