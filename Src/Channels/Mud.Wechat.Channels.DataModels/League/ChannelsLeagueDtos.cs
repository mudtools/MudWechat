// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取带货者详情信息（<c>promoter/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetPromoterRequest
{
    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，选填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }

    /// <summary>获取或设置带货者搜索参数（官方 <c>talent_search_key</c>，选填；支持带货者 appid 和视频号 finder_id）。</summary>
    [JsonPropertyName("talent_search_key")]
    public string? TalentSearchKey { get; set; }
}

/// <summary>获取带货者列表（<c>promoter/list/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetPromoterListRequest
{
    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，选填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>获取或设置单页带货者数（官方 <c>page_size</c>，必填，不超过 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }
}

/// <summary>获取带货者商品数据列表（<c>talent/getproductdatalist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetTalentProductDataListRequest
{
    /// <summary>获取或设置获取的商品数据类型（官方 <c>get_data_type</c>，必填，见 <see cref="ChannelsLeagueGetDataTypes"/>）。</summary>
    [JsonPropertyName("get_data_type")]
    public int GetDataType { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，必填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int PageIndex { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>page_size</c>，必填，不超过 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，必填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string TalentAppId { get; set; } = string.Empty;
}

/// <summary>编辑带货者（<c>talent/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsUpdateTalentRequest
{
    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，必填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string TalentAppId { get; set; } = string.Empty;

    /// <summary>获取或设置编辑操作类型（官方 <c>upd_talent_type</c>，必填，见 <see cref="ChannelsLeagueUpdTalentTypes"/>）。</summary>
    [JsonPropertyName("upd_talent_type")]
    public int UpdTalentType { get; set; }
}

/// <summary>批量新增联盟商品（<c>item/batchadd</c>）请求体。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：请求体参数表字段为 <c>talent_appids</c>，代码示例写作 <c>promoter_ids</c>
/// ——SDK 按参数表建模（官方变更日志 2026-06-17：删除 finder_ids/promoter_ids/fail_promoter_ids/
/// fail_finder_ids/info_id，新增 talent_appids/promotion_plan_id/fail_talent_appids）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddLeagueItemRequest
{
    /// <summary>获取或设置获取商品推广类别（官方 <c>type</c>，必填，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>list</c>，必填；<c>product_id</c> 不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeagueProductRatio> List { get; set; } = new();

    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>，选填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，选填；官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }

    /// <summary>获取或设置带货者 appid 列表（官方 <c>talent_appids</c>，选填）。</summary>
    [JsonPropertyName("talent_appids")]
    public List<string>? TalentAppIds { get; set; }
}

/// <summary>联盟推广商品佣金条目（官方 <c>list</c> 数组元素，<c>item/batchadd</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueProductRatio
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填，不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，必填，取值 <c>[0, 90]</c>%，操作为编辑时必填）。</summary>
    [JsonPropertyName("ratio")]
    public int Ratio { get; set; }
}

/// <summary>获取联盟推广计划详情（<c>item/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetLeagueItemRequest
{
    /// <summary>获取或设置获取商品推广类别（官方 <c>type</c>，必填，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>，选填）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }
}

/// <summary>批量新增联盟机构推广（<c>item/headsupplier/batchadd</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddHeadSupplierItemRequest
{
    /// <summary>
    /// 获取或设置机构开放唯一凭证（官方 <c>headsupplier_appid</c>，必填）——即账号 ID，
    /// 机构可在「微信小店·联盟带货机构管理平台 - 设置 - 开放信息」中获得
    /// （需要已完成机构开通，且账号没有异常状态）。
    /// </summary>
    [JsonPropertyName("headsupplier_appid")]
    public string HeadSupplierAppId { get; set; } = string.Empty;

    /// <summary>获取或设置商品列表（官方 <c>list</c>，必填；<c>product_id</c> 不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeagueHeadSupplierProduct> List { get; set; } = new();

    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>，选填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，选填；官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }
}

/// <summary>联盟机构推广商品条目（官方 <c>list</c> 数组元素，<c>item/headsupplier/batchadd</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProduct
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填，不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，必填，取值 <c>[0, 90]</c>%）。</summary>
    [JsonPropertyName("ratio")]
    public int Ratio { get; set; }

    /// <summary>获取或设置推广服务费（官方 <c>service_ratio</c>，必填，取值 <c>[0, 90]</c>%）。</summary>
    [JsonPropertyName("service_ratio")]
    public int ServiceRatio { get; set; }

    /// <summary>获取或设置机构计划类型（官方 <c>commission_type</c>，选填，见 <see cref="ChannelsLeagueHeadSupplierPlanTypes"/>）。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }
}

/// <summary>获取联盟商品列表（<c>item/list/get</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<c>page_size × page_index ≤ 5000</c>（接口限制）。<b>官方文档矛盾照录</b>：注意事项
/// 建议命中限制时改用传 <c>last_buffer</c> 顺序翻页，但官方变更日志 2026-06-17 已删除
/// <c>last_buffer</c> 等字段、当前参数表无此入参——SDK 按参数表建模（无 last_buffer）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetLeagueItemListRequest
{
    /// <summary>获取或设置单页商品数（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，选填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，选填，获取特殊推广商品时有效）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置是否需要返回满足筛选条件的商品总数（官方 <c>need_total_num</c>，选填）。</summary>
    [JsonPropertyName("need_total_num")]
    public bool? NeedTotalNum { get; set; }
}

/// <summary>更新联盟推广计划信息（<c>item/upd</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsUpdateLeagueItemRequest
{
    /// <summary>获取或设置获取商品推广类别（官方 <c>type</c>，必填，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填，可从获取联盟商品推广列表接口中获取）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>，必填）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long PromotionPlanId { get; set; }

    /// <summary>获取或设置更新操作类别（官方 <c>operate_type</c>，必填，见 <see cref="ChannelsLeagueOperateTypes"/>）。</summary>
    [JsonPropertyName("operate_type")]
    public int OperateType { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，选填，取值 <c>[0, 90]</c>%，操作为编辑时必填）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }

    /// <summary>获取或设置特殊推广信息（官方 <c>exclusive_info</c>，选填）。</summary>
    [JsonPropertyName("exclusive_info")]
    public ChannelsLeagueUpdExclusiveInfo? ExclusiveInfo { get; set; }
}

/// <summary>特殊推广信息·更新形态（官方 <c>exclusive_info</c> 对象，<c>item/upd</c>）。</summary>
/// <remarks>
/// 与 <see cref="ChannelsLeagueExclusiveInfo"/>（响应形态）分建：更新形态承载
/// <c>add_talent_appids</c>/<c>del_talent_appids</c>（各不超过 20 个），响应形态为
/// <c>talent_num</c>（推广带货者数量）——字段集不同、语义可独立演化。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueUpdExclusiveInfo
{
    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>，选填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，选填；官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }

    /// <summary>获取或设置用于新增推广达人列表（官方 <c>add_talent_appids</c>，选填，不超过 20 个）。</summary>
    [JsonPropertyName("add_talent_appids")]
    public List<string>? AddTalentAppIds { get; set; }

    /// <summary>获取或设置删除推广达人列表（官方 <c>del_talent_appids</c>，选填，不超过 20 个）。</summary>
    [JsonPropertyName("del_talent_appids")]
    public List<string>? DelTalentAppIds { get; set; }
}

/// <summary>获取商品推广计划列表（<c>promotion/plan/list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsListSpuPromotionPlansRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，必填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int PageIndex { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>page_size</c>，必填，不超过 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置推广计划类型（官方 <c>promotion_plan_type</c>，选填，见 <see cref="ChannelsLeaguePromotionPlanTypes"/>）。</summary>
    [JsonPropertyName("promotion_plan_type")]
    public int? PromotionPlanType { get; set; }
}

/// <summary>获取商品推广带货者列表（<c>promotion/talent/list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsListSpuPromotionTalentsRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，必填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int PageIndex { get; set; }

    /// <summary>获取或设置单页带货者数（官方 <c>page_size</c>，必填，不超过 10）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>，必填）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long PromotionPlanId { get; set; }

    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，选填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }
}

/// <summary>获取带货者详情信息（<c>promoter/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetPromoterResponse : ChannelsResponse
{
    /// <summary>获取或设置带货者信息（官方 <c>talent</c>）。</summary>
    [JsonPropertyName("talent")]
    public ChannelsLeagueTalent? Talent { get; set; }
}

/// <summary>带货者信息（官方 <c>talent</c> 对象，<c>promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalent
{
    /// <summary>获取或设置带货者基础信息（官方 <c>base_talent_info</c>）。</summary>
    [JsonPropertyName("base_talent_info")]
    public ChannelsLeagueTalentBaseInfo? BaseTalentInfo { get; set; }

    /// <summary>获取或设置带货者数据信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsLeagueTalentData? Data { get; set; }

    /// <summary>获取或设置带货者特征信息（官方 <c>feature_info</c>）。</summary>
    [JsonPropertyName("feature_info")]
    public ChannelsLeagueTalentFeatureInfo? FeatureInfo { get; set; }
}

/// <summary>
/// 带货者基础信息（官方 <c>base_talent_info</c> 对象）。
/// </summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：<c>promoter/get</c> 与 <c>promotion/talent/list</c> 的
/// <c>base_talent_info</c> 字段集完全一致（<c>nickname</c>/<c>head_img</c>/<c>talent_appid</c>），共用同一 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentBaseInfo
{
    /// <summary>获取或设置带货者昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置带货者头像（官方 <c>head_img</c>）。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }

    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }
}

/// <summary>带货者数据信息（官方 <c>data</c> 对象，<c>promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentData
{
    /// <summary>获取或设置累计销售额（官方 <c>total_sale_gmv</c>）。</summary>
    [JsonPropertyName("total_sale_gmv")]
    public long? TotalSaleGmv { get; set; }

    /// <summary>获取或设置近 30 日销售额（官方 <c>sale_gmv_30d</c>）。</summary>
    [JsonPropertyName("sale_gmv_30d")]
    public long? SaleGmv30d { get; set; }

    /// <summary>获取或设置累计订单数（官方 <c>total_order_num</c>）。</summary>
    [JsonPropertyName("total_order_num")]
    public long? TotalOrderNum { get; set; }

    /// <summary>获取或设置近 30 日成交订单数（官方 <c>order_num_30d</c>）。</summary>
    [JsonPropertyName("order_num_30d")]
    public long? OrderNum30d { get; set; }

    /// <summary>获取或设置近 30 日成交商品数（官方 <c>sale_product_num_30d</c>）。</summary>
    [JsonPropertyName("sale_product_num_30d")]
    public int? SaleProductNum30d { get; set; }

    /// <summary>获取或设置橱窗商品数（官方 <c>window_product_num</c>）。</summary>
    [JsonPropertyName("window_product_num")]
    public int? WindowProductNum { get; set; }
}

/// <summary>带货者特征信息（官方 <c>feature_info</c> 对象，<c>promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentFeatureInfo
{
    /// <summary>获取或设置带货者等级（官方 <c>level</c>，见 <see cref="ChannelsLeagueTalentLevels"/>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置是否被拉黑（官方 <c>is_blacklisted</c>）。</summary>
    [JsonPropertyName("is_blacklisted")]
    public bool? IsBlacklisted { get; set; }
}

/// <summary>获取带货者列表（<c>promoter/list/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetPromoterListResponse : ChannelsResponse
{
    /// <summary>获取或设置带货者总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置后面是否还有内容（官方 <c>continue_flag</c>：true 还有内容；false 已结束）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }

    /// <summary>获取或设置带货者列表信息（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeaguePromoterListItem>? List { get; set; }
}

/// <summary>带货者列表条目（官方 <c>list</c> 数组元素，<c>promoter/list/get</c>）。</summary>
/// <remarks>
/// <b>分建裁决</b>：与 <see cref="ChannelsLeagueTalentBaseInfo"/> 字段集不同
/// （仅 <c>talent_appid</c>/<c>nickname</c>，无 <c>head_img</c>），按端点分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromoterListItem
{
    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }

    /// <summary>获取或设置带货者昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}

/// <summary>获取带货者商品数据列表（<c>talent/getproductdatalist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetTalentProductDataListResponse : ChannelsResponse
{
    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置后面是否还有内容（官方 <c>continue_flag</c>：true 还有内容；false 已结束）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }

    /// <summary>获取或设置商品信息列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeagueTalentProductDataItem>? List { get; set; }
}

/// <summary>带货者商品数据条目（官方 <c>list</c> 数组元素，<c>talent/getproductdatalist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentProductDataItem
{
    /// <summary>获取或设置商品 id（官方 <c>spu_id</c>）。</summary>
    [JsonPropertyName("spu_id")]
    public long? SpuId { get; set; }

    /// <summary>获取或设置商品数据（官方 <c>product_data</c>）。</summary>
    [JsonPropertyName("product_data")]
    public ChannelsLeagueProductData? ProductData { get; set; }

    /// <summary>获取或设置带货者佣金信息（官方 <c>talent_commission_info</c>）。</summary>
    [JsonPropertyName("talent_commission_info")]
    public ChannelsLeagueTalentCommissionInfo? TalentCommissionInfo { get; set; }
}

/// <summary>
/// 带货者商品数据（官方 <c>product_data</c> 对象，<c>talent/getproductdatalist</c>）。
/// </summary>
/// <remarks>
/// <b>分建裁决</b>：与 <see cref="ChannelsLeagueTalentData"/> 字段集不同（无 <c>sale_product_num_30d</c>/
/// <c>window_product_num</c>），按端点分建、不共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueProductData
{
    /// <summary>获取或设置销售额（官方 <c>total_sale_gmv</c>）。</summary>
    [JsonPropertyName("total_sale_gmv")]
    public long? TotalSaleGmv { get; set; }

    /// <summary>获取或设置 30 天销售额（官方 <c>sale_gmv_30d</c>）。</summary>
    [JsonPropertyName("sale_gmv_30d")]
    public long? SaleGmv30d { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>total_order_num</c>）。</summary>
    [JsonPropertyName("total_order_num")]
    public long? TotalOrderNum { get; set; }

    /// <summary>获取或设置 30 天成交订单数（官方 <c>order_num_30d</c>）。</summary>
    [JsonPropertyName("order_num_30d")]
    public long? OrderNum30d { get; set; }
}

/// <summary>带货者佣金信息（官方 <c>talent_commission_info</c> 对象，<c>talent/getproductdatalist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentCommissionInfo
{
    /// <summary>获取或设置佣金类型（官方 <c>commission_type</c>，见 <see cref="ChannelsLeaguePromotionTypes"/>：1 普通计划 / 2 定向计划 / 3 专属计划）。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }

    /// <summary>获取或设置佣金率（官方 <c>ratio</c>，取值 <c>[0, 1000000]</c>，1000000 = 100%）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }
}

/// <summary>批量新增联盟商品（<c>item/batchadd</c>）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：响应参数表字段为 <c>promotion_plan_id</c>（推广计划 id），代码示例写作
/// <c>info_id</c>/<c>ratio</c>——SDK 按参数表建模（官方变更日志 2026-06-17：新增 promotion_plan_id、删除 info_id）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddLeagueItemResponse : ChannelsResponse
{
    /// <summary>获取或设置逐商品添加结果（官方 <c>result_info_list</c>）。</summary>
    [JsonPropertyName("result_info_list")]
    public List<ChannelsLeagueItemResultInfo>? ResultInfoList { get; set; }
}

/// <summary>联盟商品批量操作结果条目（官方 <c>result_info_list</c> 数组元素，<c>item/batchadd</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueItemResultInfo
{
    /// <summary>获取或设置单个商品添加错误码（官方 <c>errcode</c>；单条目级错误码，非顶层响应码）。</summary>
    [JsonPropertyName("errcode")]
    public int? Errcode { get; set; }

    /// <summary>获取或设置单个商品错误信息（官方 <c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }

    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>；官方示例写作 info_id——参数表照录）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置推广失败带货者列表（官方 <c>fail_talent_appids</c>）。</summary>
    [JsonPropertyName("fail_talent_appids")]
    public List<string>? FailTalentAppIds { get; set; }
}

/// <summary>获取联盟推广计划详情（<c>item/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetLeagueItemResponse : ChannelsResponse
{
    /// <summary>获取或设置推广计划信息（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public ChannelsLeagueItem? Item { get; set; }
}

/// <summary>联盟推广计划信息（官方 <c>item</c> 对象，<c>item/get</c>）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：<c>exclusive_info</c> 参数表字段为 <c>promotion_plan_id</c>/<c>begin_time</c>/
/// <c>end_time</c>/<c>is_forerver</c>/<c>talent_num</c>，代码示例写作 <c>info_id</c>/<c>finder_ids</c>/
/// <c>finder_num</c>/<c>finder_info_list</c>；<c>ext_info</c> 参数表为 <c>is_banned</c>、示例为
/// <c>is_sale_forbidden</c>——SDK 均按参数表建模（官方变更日志 2026-06-17：新增 promotion_plan_id、删除 info_id）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueItem
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品推广类别（官方 <c>type</c>，见 <see cref="ChannelsLeaguePromotionTypes"/>：1 普通推广商品 / 2 定向推广商品 / 3 专属推广商品——与请求侧 <see cref="ChannelsLeaguePlanQueryTypes"/> 是两套枚举）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置商品推广状态（官方 <c>status</c>，见 <see cref="ChannelsLeagueItemStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，取值 <c>[0, 90]</c>%）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }

    /// <summary>获取或设置推广信息（官方 <c>exclusive_info</c>，特殊推广计划形态）。</summary>
    [JsonPropertyName("exclusive_info")]
    public ChannelsLeagueExclusiveInfo? ExclusiveInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsLeagueItemExtInfo? ExtInfo { get; set; }
}

/// <summary>联盟特殊推广信息·响应形态（官方 <c>exclusive_info</c> 对象，<c>item/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueExclusiveInfo
{
    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }

    /// <summary>获取或设置推广带货者数量（官方 <c>talent_num</c>）。</summary>
    [JsonPropertyName("talent_num")]
    public int? TalentNum { get; set; }
}

/// <summary>联盟推广计划扩展信息（官方 <c>ext_info</c> 对象，<c>item/get</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：参数表字段为 <c>is_banned</c>，代码示例为 <c>is_sale_forbidden</c>——按参数表建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueItemExtInfo
{
    /// <summary>获取或设置是否被官方封禁（官方 <c>is_banned</c>）。</summary>
    [JsonPropertyName("is_banned")]
    public bool? IsBanned { get; set; }
}

/// <summary>批量新增联盟机构推广（<c>item/headsupplier/batchadd</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddHeadSupplierResponse : ChannelsResponse
{
    /// <summary>获取或设置逐商品添加结果（官方 <c>result_info_list</c>）。</summary>
    [JsonPropertyName("result_info_list")]
    public List<ChannelsLeagueHeadSupplierResultInfo>? ResultInfoList { get; set; }
}

/// <summary>联盟机构推广批量操作结果条目（官方 <c>result_info_list</c> 数组元素，<c>item/headsupplier/batchadd</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierResultInfo
{
    /// <summary>获取或设置单个商品添加错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int? Errcode { get; set; }

    /// <summary>获取或设置单个商品错误信息（官方 <c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }
}

/// <summary>获取联盟商品列表（<c>item/list/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetLeagueItemListResponse : ChannelsResponse
{
    /// <summary>获取或设置商品信息（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public List<ChannelsLeagueListProduct>? Items { get; set; }

    /// <summary>获取或设置是否还有剩余商品（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>联盟商品列表条目（官方 <c>items</c> 数组元素，<c>item/list/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueListProduct
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品状态（官方 <c>status</c>，见 <see cref="ChannelsLeagueItemStatuses"/>，与推广计划详情同值域）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>更新联盟推广计划信息（<c>item/upd</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsUpdateLeagueItemResponse : ChannelsResponse
{
    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置更新失败的达人 appid 列表（官方 <c>fail_talent_appids</c>）。</summary>
    [JsonPropertyName("fail_talent_appids")]
    public List<string>? FailTalentAppIds { get; set; }
}

/// <summary>获取商品推广计划列表（<c>promotion/plan/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsListSpuPromotionPlansResponse : ChannelsResponse
{
    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置后面是否还有内容（官方 <c>continue_flag</c>：true 还有内容；false 已结束）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }

    /// <summary>获取或设置商品信息列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeaguePromotionPlanItem>? List { get; set; }
}

/// <summary>推广计划条目（官方 <c>list</c> 数组元素，<c>promotion/plan/list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromotionPlanItem
{
    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置推广计划类型（官方 <c>promotion_plan_type</c>，见 <see cref="ChannelsLeaguePromotionPlanTypes"/>）。</summary>
    [JsonPropertyName("promotion_plan_type")]
    public int? PromotionPlanType { get; set; }
}

/// <summary>获取商品推广带货者列表（<c>promotion/talent/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsListSpuPromotionTalentsResponse : ChannelsResponse
{
    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置后面是否还有内容（官方 <c>continue_flag</c>：true 还有内容；false 已结束）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }

    /// <summary>获取或设置商品信息列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeaguePromotionTalentItem>? List { get; set; }
}

/// <summary>商品推广带货者条目（官方 <c>list</c> 数组元素，<c>promotion/talent/list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromotionTalentItem
{
    /// <summary>获取或设置带货者基础信息（官方 <c>base_talent_info</c>，与 <c>promoter/get</c> 同形态 ⇒ 共用 <see cref="ChannelsLeagueTalentBaseInfo"/>）。</summary>
    [JsonPropertyName("base_talent_info")]
    public ChannelsLeagueTalentBaseInfo? BaseTalentInfo { get; set; }

    /// <summary>获取或设置带货者佣金信息（官方 <c>commission_info</c>）。</summary>
    [JsonPropertyName("commission_info")]
    public ChannelsLeaguePromotionCommissionInfo? CommissionInfo { get; set; }

    /// <summary>获取或设置带货者推广状态（官方 <c>talent_relation_status</c>，见 <see cref="ChannelsLeagueTalentRelationStatuses"/>）。</summary>
    [JsonPropertyName("talent_relation_status")]
    public int? TalentRelationStatus { get; set; }
}

/// <summary>
/// 推广佣金信息（官方 <c>commission_info</c> 对象，<c>promotion/talent/list</c>）。
/// </summary>
/// <remarks>
/// <b>分建裁决</b>：与 <see cref="ChannelsLeagueTalentCommissionInfo"/> 字段集不同
/// （计划 id + 计划类型 + 佣金率，无 <c>commission_type</c>），按端点分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeaguePromotionCommissionInfo
{
    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置推广计划类型（官方 <c>promotion_plan_type</c>，见 <see cref="ChannelsLeaguePromotionPlanTypes"/>）。</summary>
    [JsonPropertyName("promotion_plan_type")]
    public int? PromotionPlanType { get; set; }

    /// <summary>获取或设置推广佣金率（官方 <c>ratio</c>，取值 <c>[0, 1000000]</c>%，1000000 = 100%）。</summary>
    [JsonPropertyName("ratio")]
    public int? Ratio { get; set; }
}

/// <summary>请求侧「获取商品推广类别」常量（官方请求体 <c>type</c> 枚举值，<c>item/batchadd</c> / <c>item/get</c> / <c>item/upd</c>）。</summary>
/// <remarks>与计划/佣金侧「普通/定向/专属」三分类（<see cref="ChannelsLeaguePromotionTypes"/>，值 1/2/3）是两套不同枚举，勿混用。</remarks>
public static class ChannelsLeaguePlanQueryTypes
{
    /// <summary>全部带货者计划。</summary>
    public const int All = 100;

    /// <summary>指定带货者计划。</summary>
    public const int Designated = 300;
}

/// <summary>商品推广类别 / 佣金类型常量（官方响应 <c>item.type</c> 与 <c>talent_commission_info.commission_type</c> 枚举值，值域同为 1/2/3）。</summary>
public static class ChannelsLeaguePromotionTypes
{
    /// <summary>普通推广商品（佣金类型侧为「普通计划」）。</summary>
    public const int Normal = 1;

    /// <summary>定向推广商品（佣金类型侧为「定向计划」）。</summary>
    public const int Targeted = 2;

    /// <summary>专属推广商品（佣金类型侧为「专属计划」）。</summary>
    public const int Exclusive = 3;
}

/// <summary>商品推广状态常量（官方 <c>status</c> 枚举值，<c>item/get</c> 与 <c>item/list/get</c> 同值域）。</summary>
public static class ChannelsLeagueItemStatuses
{
    /// <summary>已上架推广。</summary>
    public const int Listed = 1;

    /// <summary>已下架推广。</summary>
    public const int Delisted = 2;

    /// <summary>已删除。</summary>
    public const int Deleted = 4;

    /// <summary>未达到准入标准。</summary>
    public const int BelowThreshold = 5;

    /// <summary>商品下架终止推广（官方 2026-07-15 新增）。</summary>
    public const int TerminatedByProductDelisted = 7;

    /// <summary>计划过期终止推广（官方 2026-07-15 新增）。</summary>
    public const int TerminatedByPlanExpired = 8;

    /// <summary>商品被封禁下架终止推广（官方 2026-07-15 新增）。</summary>
    public const int TerminatedByProductBanned = 9;

    /// <summary>待生效。</summary>
    public const int PendingEffective = 10;
}

/// <summary>编辑带货者操作类型常量（官方 <c>upd_talent_type</c> 枚举值，<c>talent/update</c>）。</summary>
public static class ChannelsLeagueUpdTalentTypes
{
    /// <summary>拉黑带货者。</summary>
    public const int Blacklist = 1;

    /// <summary>取消拉黑带货者。</summary>
    public const int Unblacklist = 2;
}

/// <summary>更新操作类别常量（官方 <c>operate_type</c> 枚举值，<c>item/upd</c>）。</summary>
public static class ChannelsLeagueOperateTypes
{
    /// <summary>编辑并上架。</summary>
    public const int EditAndList = 1;

    /// <summary>下架。</summary>
    public const int Delist = 2;

    /// <summary>上架。</summary>
    public const int List = 4;
}

/// <summary>推广计划类型常量（官方 <c>promotion_plan_type</c> 枚举值，<c>promotion/plan/list</c> 与 <c>promotion/talent/list</c>）。</summary>
public static class ChannelsLeaguePromotionPlanTypes
{
    /// <summary>全部带货者。</summary>
    public const int All = 100;

    /// <summary>圈选带货者。</summary>
    public const int Selected = 200;

    /// <summary>指定带货者。</summary>
    public const int Designated = 300;
}

/// <summary>机构计划类型常量（官方 <c>commission_type</c> 枚举值，<c>item/headsupplier/batchadd</c>）。</summary>
public static class ChannelsLeagueHeadSupplierPlanTypes
{
    /// <summary>商家指定达人佣金。</summary>
    public const int MerchantDesignated = 0;

    /// <summary>机构指定达人佣金【总包计划类型】。</summary>
    public const int OrgDesignated = 1;
}

/// <summary>带货者等级常量（官方 <c>level</c> 枚举值，<c>promoter/get</c>）。</summary>
public static class ChannelsLeagueTalentLevels
{
    /// <summary>带货新人。</summary>
    public const int Rookie = 0;

    /// <summary>一星带货者。</summary>
    public const int OneStar = 1;

    /// <summary>二星带货者。</summary>
    public const int TwoStar = 2;

    /// <summary>三星带货者。</summary>
    public const int ThreeStar = 3;

    /// <summary>四星带货者。</summary>
    public const int FourStar = 4;

    /// <summary>五星带货者。</summary>
    public const int FiveStar = 5;
}

/// <summary>带货者推广状态常量（官方 <c>talent_relation_status</c> 枚举值，<c>promotion/talent/list</c>）。</summary>
public static class ChannelsLeagueTalentRelationStatuses
{
    /// <summary>推广中。</summary>
    public const int Promoting = 1;

    /// <summary>已移除。</summary>
    public const int Removed = 4;

    /// <summary>已被拉黑。</summary>
    public const int Blacklisted = 10;
}

/// <summary>获取的商品数据类型常量（官方 <c>get_data_type</c> 枚举值，<c>talent/getproductdatalist</c>）。</summary>
public static class ChannelsLeagueGetDataTypes
{
    /// <summary>近 30 天日成交商品数据。</summary>
    public const int Last30DaysDaily = 1;

    /// <summary>当前橱窗商品数据。</summary>
    public const int CurrentWindow = 2;
}
