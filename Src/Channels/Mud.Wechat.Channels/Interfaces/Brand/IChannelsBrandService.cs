// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Brand;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「品牌资质」域 SDK（8 端点：品牌资质新增 / 更新 / 删除 /
/// 撤回审核 / 申请详情 / 申请列表 / 生效中列表 / 品牌库查询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由前缀并存（官方历史前缀，照抄原文不得改写）</b>：本域同时使用 <c>/shop/ec/brand/*</c>（3 端点：
/// 新增 / 品牌库 / 撤回审核）与 <c>/channels/ec/brand/*</c>（5 端点：删除 / 详情 / 列表 / 更新 / 生效中列表）。
/// </para>
/// <para>
/// <b>brand_id 类型不一致（官方照录）</b>：品牌库列表（<c>brand/all</c>）返回 <c>brand_id</c> 为
/// <b>string</b>；而新增 / 更新请求体 <c>brand.brand_id</c> 与详情响应 <c>brand.brand_id</c> 为
/// <b>number</b>；删除 / 撤回 / 详情请求体的 <c>brand_id</c> 又为 <b>string</b>——SDK 逐端点照抄官方参数表，不做归一。
/// </para>
/// <para>
/// <b>申请记录 vs 生效资质</b>：<c>brand/get</c> / <c>brand/list/get</c> 返回的是商家提交的申请记录（草稿数据）；
/// 要获取线上生效中的资质须用 <c>brand/valid/list/get</c>（仅返回状态 3 生效中 / 5 即将过期）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Brand", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsBrandService
{
    /// <summary>
    /// 新增品牌资质（<c>addbrandlogic</c>，提交品牌资质审核单）。
    /// </summary>
    /// <param name="request">新增请求（<c>brand</c> 必填，品牌详情）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 ID（<c>audit_id</c>，提交审核成功后返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_addbrandlogic"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。商标类型枚举见 <see cref="ChannelsBrandTradeMarkSymbols"/>、
    /// 授权类型枚举见 <see cref="ChannelsBrandGrantTypes"/>。</para>
    /// <para>
    /// 官方业务限制：①<b>每个品牌只能调用一次新增接口</b>——已有申请记录的品牌须改用「更新品牌资质」
    /// （updatebrandlogic）；②新增前需先通过「获取品牌库列表」获取品牌库品牌及编号；
    /// ③资质文件须先用「上传资质图片」接口上传获取 file_id（注册证最多 1 张、变更/续展证明最多 5 张、
    /// 销售授权书最多 9 张、权利人证件照最多 2 张）；④授权品牌建议使用分级授权书结构（<c>use_split_grant_info</c> 大于 0 即使用）。
    /// </para>
    /// <para>
    /// 官方错误码（节选）：<c>10020080</c>（提交的资质数量超出限制）/ <c>10020084</c>（品牌名称与品牌库 ID 不匹配）/
    /// <c>10020085</c>（该品牌申请正在审核中，不要重复提交）/ <c>10020602</c>（品牌授权类型错误）/
    /// <c>10020603</c>（品牌商标类型错误）/ <c>10020605</c>（授权等级须为 1-3）/ <c>10020606</c>（品牌不支持非 R 标）/
    /// <c>10020609</c>（商标分类号须为 1-45）/ <c>10020610</c>（品牌申请已存在）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/brand/add")]
    Task<ChannelsBrandAuditIdResponse> AddBrandAsync(
        [Body] ChannelsAddBrandRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取品牌库列表（<c>getallbrandslogic</c>，新增品牌资质的前置查询）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填；<c>next_key</c> 选填翻页上下文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>品牌库品牌列表（<c>brands</c>：brand_id / 中文名 / 英文名）、翻页上下文与是否还有下一页。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_getallbrandslogic"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。官方本端点 <c>brands[].brand_id</c> 为 <b>string</b> 型（照录）。</para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；<c>page_size</c> 默认 10、不超过 50；
    /// 新增品牌资质前需先通过本接口获取目标品牌的 brand_id。</para>
    /// <para>官方错误码：<c>10020050</c>（无权限调用该 api）/ <c>10020055</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/brand/all")]
    Task<ChannelsGetAllBrandsResponse> GetAllBrandsAsync(
        [Body] ChannelsGetAllBrandsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤回品牌资质审核（<c>cancelauditbrandlogic</c>）。
    /// </summary>
    /// <param name="request">撤回请求（<c>brand_id</c>/<c>audit_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_cancelauditbrandlogic"/></para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。</para>
    /// <para>官方业务限制：处于审核中状态的品牌资质，需先撤回审核后才能重新提交更新。</para>
    /// <para>官方错误码：<c>10020081</c>（品牌的申请记录不存在）等；其余走通用错误码。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/ec/brand/audit/cancel")]
    Task<ChannelsResponse> CancelBrandAuditAsync(
        [Body] ChannelsCancelBrandAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除品牌资质（<c>deletebrandlogic</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>brand_id</c> 必填，品牌库中的品牌编号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_deletebrandlogic"/></para>
    /// <para>官方契约：<b>POST</b>；本端点支持云调用（<c>channels.ec.brand.delete</c>）。</para>
    /// <para><b>官方业务限制（破坏性操作警示）</b>：删除品牌资质后，对应品牌关联的商品将会被下架。</para>
    /// <para>官方错误码：<c>10020081</c>（品牌的申请记录不存在）等；其余走通用错误码。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/brand/delete")]
    Task<ChannelsResponse> DeleteBrandAsync(
        [Body] ChannelsDeleteBrandRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取品牌资质申请详情（<c>getbrandlogic</c>，按品牌 id 查申请单）。
    /// </summary>
    /// <param name="request">查询请求（<c>brand_id</c> 必填，品牌库中的品牌编号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>品牌资质详情（<c>brand</c>：商标信息 / 注册信息 / 申请信息 / 授权信息 / 状态 / 审核结果）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_getbrandlogic"/></para>
    /// <para>官方契约：<b>POST</b>；本端点支持云调用（<c>channels.ec.brand.get</c>）。品牌资质状态枚举见
    /// <see cref="ChannelsBrandStatuses"/>；响应 <c>brand</c> 与两个列表接口的元素字段集一致 ⇒ 共用
    /// <see cref="ChannelsBrandApplicationInfo"/>。</para>
    /// <para>
    /// 官方业务限制：①返回的资质文件为 file_id 格式，开发者需自行维护 file_id 与文件的映射关系；
    /// ②审核失败时可通过 <c>audit_result.reject_reason</c> 查看不通过原因。
    /// </para>
    /// <para>官方错误码：<c>10020081</c>（品牌的申请记录不存在）等；其余走通用错误码。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/brand/get")]
    Task<ChannelsGetBrandResponse> GetBrandAsync(
        [Body] ChannelsGetBrandRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取品牌资质申请列表（<c>getbrandlistlogic</c>，分页拉取申请单，可按状态筛选）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填；<c>status</c> 选填状态筛选；<c>next_key</c> 选填翻页游标）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请单列表（<c>brands</c>）、符合条件的总数（<c>total_num</c>）与翻页游标（<c>next_key</c>，返回空表示最后一页）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_getbrandlistlogic"/></para>
    /// <para>官方契约：<b>POST</b>；本端点支持云调用（<c>channels.ec.brand.list.get</c>）。
    /// 状态筛选枚举见 <see cref="ChannelsBrandListFilterStatuses"/>（与品牌资质状态枚举语义不同，勿混用）。</para>
    /// <para>
    /// 官方业务限制：①返回的是商家提交的申请记录（草稿数据）——要获取线上生效中的资质，请使用
    /// 「获取生效中的品牌资质列表」接口；②资质文件以 file_id 格式返回，开发者需自行维护映射关系；
    /// ③<c>page_size</c> 默认 10、不超过 50；<c>status</c> 不传或传 0 返回全部。
    /// </para>
    /// <para>官方错误码：<c>10020050</c>（无权限调用该 api）/ <c>10020055</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/brand/list/get")]
    Task<ChannelsGetBrandListResponse> GetBrandListAsync(
        [Body] ChannelsGetBrandListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新品牌资质（<c>updatebrandlogic</c>，调用即自动提交审核）。
    /// </summary>
    /// <param name="request">更新请求（<c>brand</c> 必填，品牌详情；与新增请求的 brand 对象字段集一致 ⇒ 共用 DTO）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 ID（<c>audit_id</c>，提交审核成功后返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_updatebrandlogic"/></para>
    /// <para>官方契约：<b>POST</b>；本端点支持云调用（<c>channels.ec.brand.update</c>）。</para>
    /// <para>
    /// 官方业务限制：①调用即自动提交审核；审核中品牌不可重复提交，需先调用「撤回品牌资质审核」；
    /// ②所有资质文件需先通过「上传资质图片」接口上传；③授权品牌建议使用分级授权书结构
    /// （<c>use_split_grant_info</c> 区分，使用时兼容填充旧结构）。
    /// </para>
    /// <para>官方错误码与「新增品牌资质」一致（节选见 <see cref="AddBrandAsync"/>）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>129</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/brand/update")]
    Task<ChannelsBrandAuditIdResponse> UpdateBrandAsync(
        [Body] ChannelsUpdateBrandRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取生效中的品牌资质列表（<c>getvalidbrandlistlogic</c>，发布商品选品牌的数据源）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填；<c>next_key</c> 选填翻页游标）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生效中的品牌资质列表（<c>brands</c>，仅状态 3 生效中 / 5 即将过期）、总数与翻页游标。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/brand/api_getvalidbrandlistlogic"/></para>
    /// <para>官方契约：<b>POST</b>；本端点支持云调用（<c>channels.ec.brand.valid.list.get</c>）。</para>
    /// <para>
    /// 官方业务限制：①仅返回状态为生效中（3）和即将过期（5）的品牌资质；②即将过期的资质不影响商品正常售卖，
    /// 但建议尽快续期；③资质文件为 file_id 格式，开发者需自行维护映射关系。
    /// </para>
    /// <para>官方错误码：<c>10020050</c>（无权限调用该 api）/ <c>10020055</c>（参数有误）。</para>
    /// <para><b>第三方平台代调用所属权限集 id 仅为 <c>129</c></b>（本域其它端点为 85/129/192，注意差异）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/brand/valid/list/get")]
    Task<ChannelsGetValidBrandListResponse> GetValidBrandListAsync(
        [Body] ChannelsGetValidBrandListRequest request,
        CancellationToken cancellationToken = default);
}
