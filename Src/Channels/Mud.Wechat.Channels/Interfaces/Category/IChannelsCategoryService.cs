// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Category;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「类目管理」域 SDK（10 端点：类目 8 + 类目规则 2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由前缀并存（官方历史前缀，照抄原文不得改写）</b>：本域同时使用 <c>/shop/ec/category/*</c>（9 端点）
/// 与 <c>/channels/ec/category/*</c>（1 端点：申请类目 <c>category/add</c>）。
/// </para>
/// <para>
/// <b><c>getcategoryrule</c> 双语义同路由</b>：官方清单中该路由出现两次——「获取保证金类目规则」
/// （<c>rule_id=1</c>，返回 <c>deposit</c>）与「获取发货方式类目规则」（<c>rule_id=2</c>，返回 <c>list</c>），
/// 同路由只计 1 端点，SDK 以 <c>rule_id</c> Query 参数分流，两用共用一个方法
/// <see cref="GetCategoryRuleAsync"/>（规则 id 常量见 <see cref="ChannelsCategoryRuleIds"/>）。
/// </para>
/// <para>
/// <b>新旧类目树</b>：<c>getallcategory</c> 同时返回 <c>cats</c>（旧三级树）与 <c>cats_v2</c>（新多级树）；
/// 申请类目设了 <c>cats_v2</c> 则优先读取，未设才回落 <c>level1/level2/level3</c>。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Category", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsCategoryService
{
    /// <summary>
    /// 获取所有类目（<c>getallcategory</c>，全部类目信息 + 类目资质信息 + 商品资质信息）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 类目信息列表（<c>cats</c> 旧三级树 + <c>cats_v2</c> 新多级树；每项含类目 <c>cat</c>、
    /// 类目资质 <c>qua</c>、商品资质 <c>product_qua</c>（将废弃）/ <c>product_qua_list</c>、品牌资质 <c>brand_qua</c>）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_getallcategory"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方变更日志：2026-09-15 HTTPS 调用由 GET 改为 POST）；<b>不支持云调用</b>。</para>
    /// <para>
    /// 官方业务限制：①接口应在服务器端调用，不可在前端（小程序、网页、APP 等）直接调用；
    /// ②证照组要求（<c>qua.cert_group_list</c>，仅 <c>cats</c> 旧树携带）是「申请类目」的提交依据——
    /// 每个类目有多个证照组要求，须提交全部证照组，每组内多个证照只需选一个提交；
    /// ③<c>field_key</c> 枚举：<c>audit_license_no</c>（许可证编号）/ <c>audit_license_registration_no</c>（备案编号）/
    /// <c>audit_license_validty_period</c>（有效期，官方历史拼写照录）。
    /// </para>
    /// <para>官方错误码：<c>9401020</c>（非法参数）；其余走通用错误码。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/all")]
    Task<ChannelsGetAllCategoryResponse> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取类目信息（<c>getcategorydetail</c>，按叶子类目 id 查类目属性 / 交易费 / 折扣规则等）。
    /// </summary>
    /// <param name="request">查询请求（<c>cat_id</c> 必填，叶子类目 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 类目信息（<c>info</c> 类目 id / 名称）、类目属性（<c>attr</c>：虚拟发货 / 定向准入 / 预售 / 7 天无理由 /
    /// 定准品牌 / 保证金 / 交易佣金 / 折扣规则 / 价格下限 / 尺码表等）与资质信息（<c>product_qua_list</c>）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_getcategorydetail"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。</para>
    /// <para>
    /// 官方业务限制：①接口应在服务器端调用，不可在前端直接调用；②产品属性 / 销售属性官方建议改用
    /// 「获取类目下商品发布规则」接口（<see cref="GetCategoryProductRuleAsync"/>，2026-05-28 变更日志）；
    /// ③<c>attr.type</c>（旧属性类型）与 <c>type_v2</c>（7 种）的兼容规则按官方说明处理（多选降级单选、
    /// 整数 / 小数 / 带单位类型按文本输入并做转换或正则匹配）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>9401020</c>（参数有误）/ <c>10020063</c>（无效的类目 id）/ <c>10020079</c>（无效的三级类目 id）/
    /// <c>10020105</c>（当前账号既非企业亦非个体工商户，也许是尚未注册完成）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/detail")]
    Task<ChannelsGetCategoryDetailResponse> GetCategoryDetailAsync(
        [Body] ChannelsGetCategoryDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请类目（<c>addcategory</c>，上传类目资质并创建审核单）。
    /// </summary>
    /// <param name="request">申请请求（<c>category_info</c> 必填；类目树与证照组规则见 <see cref="ChannelsCategoryInfoApply"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 id（<c>audit_id</c>）；审核完成后通过官方「类目审核结果通知」回调通知。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_addcategory"/></para>
    /// <para>官方契约：<b>POST</b>；本端点支持云调用（<c>channels.ec.category.add</c>）。</para>
    /// <para>
    /// 官方业务限制：①图片必须用「上传资质图片」接口获取的 file_id，资质材料最多 10 张；
    /// ②<c>is_new_apply_cat</c> 须设为 <c>true</c> 并用 <c>license_group_list</c>（证照组）提交——
    /// 过渡期兼容旧 <c>certificate</c> 提交（缺省或 <c>false</c> 按旧资质处理，官方已标废弃）；
    /// ③每个类目有多个证照组要求，须提交全部证照组，每组内多个证照只需选一个；
    /// ④设了 <c>cats_v2</c> 则优先读取（多级结构），未设则用 <c>level1/level2/level3</c>（旧三级结构），
    /// <c>cats_v2</c> 数组下标顺序与一至 N 级类目严格一致；
    /// ⑤品牌定向准入类目（<c>attr.is_limit_brand</c> 为 <c>true</c>）时 <c>brand_list</c> 必传。
    /// </para>
    /// <para>
    /// 官方错误码（节选）：<c>9700234</c>（证照组要求没对齐）/ <c>9700250</c>（请使用新版申请协议）/
    /// <c>10020062</c>（资质材料超限，最多 10 张）/ <c>10020063</c>（不合法的类目 id）/ <c>10020064</c>（该类目审核中，无需重复提交）/
    /// <c>10020087</c>（不合法的 fileid）/ <c>10020094</c>（必须使用上传资质接口获取 fileid）/
    /// <c>10020127</c>（当前选择类目必须提交品牌）/ <c>10020232</c>/<c>10020233</c>（品牌力不满足 / 评定中）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/category/add")]
    Task<ChannelsAddCategoryResponse> AddCategoryAsync(
        [Body] ChannelsAddCategoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺的类目审核单列表（<c>getbizcatflowlist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>is_filter_status</c>/<c>offset</c>/<c>limit</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单列表（<c>list</c>：类目 id / 审核单 id / 状态 / 审核材料 / 驳回原因等）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_getbizcatflowlist"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。审核单状态枚举见 <see cref="ChannelsCategoryAuditStatuses"/>。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>9401020</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/getbizcatflowlist")]
    Task<ChannelsGetBizCatFlowListResponse> GetBizCatFlowListAsync(
        [Body] ChannelsGetBizCatFlowListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺的类目审核单详情（<c>getbizcatflowdetail</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>audit_id</c> 必填，审核单 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单详情（<c>info</c>：类目 id / 状态 / 审核材料 <c>audit_info</c> / 审核时间 / 驳回原因）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_getbizcatflowdetail"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。响应 <c>info</c> 与列表接口 <c>list</c> 元素字段集一致 ⇒ 共用 <see cref="ChannelsCategoryAuditFlow"/>。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>9401020</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/getbizcatflowdetail")]
    Task<ChannelsGetBizCatFlowDetailResponse> GetBizCatFlowDetailAsync(
        [Body] ChannelsGetBizCatFlowDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺的类目权限列表（<c>get_category_relation_list</c>，路由含下划线照抄官方）。
    /// </summary>
    /// <param name="request">查询请求（<c>is_filter_status</c> 必填；<c>status</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>类目权限列表（<c>list</c>：类目 id / 状态 / 失效原因 / 生效与失效时间 / 资质 id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_getcategoryrelationlist"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。类目状态枚举见 <see cref="ChannelsCategoryRelationStatuses"/>。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>9401020</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/get_category_relation_list")]
    Task<ChannelsGetCategoryRelationListResponse> GetCategoryRelationListAsync(
        [Body] ChannelsGetCategoryRelationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺的类目权限详情（<c>get_category_relation_detail</c>，路由含下划线照抄官方）。
    /// </summary>
    /// <param name="request">查询请求（<c>category_id</c> 必填，类目 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该类目下的权限详情（状态 / 失效原因 / 审核材料 <c>audit_info</c> / 生效与失效时间 / 资质 id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_getcategoryrelationdetail"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。类目状态枚举见 <see cref="ChannelsCategoryRelationStatuses"/>；
    /// <c>audit_info</c> 与审核单两接口字段集一致 ⇒ 共用 <see cref="ChannelsCategoryAuditInfo"/>。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>9401020</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/get_category_relation_detail")]
    Task<ChannelsGetCategoryRelationDetailResponse> GetCategoryRelationDetailAsync(
        [Body] ChannelsGetCategoryRelationDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销类目审核（<c>cancelauditcategory</c>，按审核单 id 撤销）。
    /// </summary>
    /// <param name="request">撤销请求（<c>audit_id</c> 必填——官方此端点为 string 类型，与审核单详情接口的 number 型不一致，照抄官方）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-category/api_cancelauditcategory"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。成功后触发官方「类目审核结果通知」（ProductCategoryAudit）。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。</para>
    /// <para>官方错误码：<c>10020006</c>（不合法的审核 id）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/audit/cancel")]
    Task<ChannelsResponse> CancelCategoryAuditAsync(
        [Body] ChannelsCancelCategoryAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取类目规则（<c>getcategoryrule</c>——官方双语义同路由端点：<c>rule_id=1</c> 保证金规则 /
    /// <c>rule_id=2</c> 发货方式规则；规则 id 常量见 <see cref="ChannelsCategoryRuleIds"/>）。
    /// </summary>
    /// <param name="ruleId">规则 id（官方 <c>rule_id</c>，URL 参数，必填）：保证金传 1、发货方式传 2。</param>
    /// <param name="request">查询请求（<c>category_id</c> 必填，类目 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 按 <c>rule_id</c> 二分：<c>rule_id=1</c>（保证金）返回 <c>deposit</c>（单位：分）；
    /// <c>rule_id=2</c>（发货方式）返回 <c>list</c>（<c>deliver_type</c> 发货方式 × <c>acct_type</c> 支持的账号类型）。
    /// </returns>
    /// <remarks>
    /// <para>
    /// 官方文档（保证金）：<see href="https://developers.weixin.qq.com/doc/store/shop/API/category-rule/api_get_category_rule"/>；
    /// 官方文档（发货方式）：<see href="https://developers.weixin.qq.com/doc/store/shop/API/category-rule/api_get_delivery_method_category_rule"/>。
    /// </para>
    /// <para>
    /// <b>双语义合并建模（同路由只计 1 端点）</b>：官方清单中 <c>/shop/ec/category/getcategoryrule</c> 出现两次，
    /// 请求体形态完全一致（仅 <c>category_id</c>），差异只在 URL 参数 <c>rule_id</c>（1 保证金 / 2 发货方式）
    /// 与响应形态（<c>deposit</c> 标量 / <c>list</c> 数组）⇒ SDK 合并为一个方法，响应 DTO 以可空超集承载两种语义
    /// （<see cref="ChannelsGetCategoryRuleResponse"/>），调用方按传入的 <c>ruleId</c> 取对应字段。
    /// </para>
    /// <para>官方契约：<b>POST</b> + <c>rule_id</c> URL 参数；<b>不支持云调用</b>。发货方式 / 账号类型枚举见
    /// <see cref="ChannelsCategoryDeliverTypes"/>、<see cref="ChannelsCategoryAcctTypes"/>。</para>
    /// <para>官方业务限制：「获取店铺在这个类目下的保证金，rule_id 请填写 1」；接口应在服务器端调用；两种语义官方均注明「本接口无特殊注意事项」。</para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/getcategoryrule")]
    Task<ChannelsGetCategoryRuleResponse> GetCategoryRuleAsync(
        [Query("rule_id")] int ruleId,
        [Body] ChannelsGetCategoryRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取类目下商品发布规则（<c>getcategoryproductrule</c>，商品属性 / 销售属性 / 资质 / 预售规则等发品约束）。
    /// </summary>
    /// <param name="request">查询请求（<c>cat_id</c>/<c>release_mode</c> 必填；<c>brand_id</c> 选填，传入时返回 <c>brand_info</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 发布规则（<c>product_attr_list</c>/<c>sale_attr_list</c> 属性及必填规则、<c>product_qua_list</c> 资质、
    /// <c>floor_price</c> 价格下限、<c>size_chart</c> 尺码表、<c>extra_service_list</c> 额外服务、
    /// <c>presale_rule</c> 预售规则、<c>is_directed_category</c> 定向类目、<c>restricted_brand_list</c> 可售品牌范围等）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/category-rule/api_getcategoryproductrule"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。发布模式枚举见 <see cref="ChannelsCategoryReleaseModes"/>；
    /// 属性类型 <c>type_v2</c> 枚举见 <see cref="ChannelsCategoryAttrTypes"/>。</para>
    /// <para>
    /// 官方业务限制：①建议调用前先调「发品前校验」接口，确保有对应类目的发品权限；
    /// ②接口根据店铺情况及所选类目返回规则；③本接口为「获取类目信息」中产品属性 / 销售属性的最新替代来源
    /// （官方 2026-05-28 变更日志建议）。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/category/getcategoryproductrule")]
    Task<ChannelsGetCategoryProductRuleResponse> GetCategoryProductRuleAsync(
        [Body] ChannelsGetCategoryProductRuleRequest request,
        CancellationToken cancellationToken = default);
}
