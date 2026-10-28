// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.DynamicCreatives;

/// <summary>
/// 组件化创意（<c>dynamic_creatives/*</c>）的应答条目（官方 <c>data.list[]</c> 元素，2026-10-11 L3 核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>组件体系是官方 union 的容器</b>：<see cref="CreativeComponents"/> 的 44 个组件键
/// （<c>title</c> / <c>image</c> / <c>video</c> …）是官方<b>固定</b>键集，每个键是一个
/// <c>struct[]</c>，元素为 <c>{component_id, value, is_deleted}</c>；而 <c>value</c>
/// 逐组件形状不同（标题是 <c>{content}</c>、图片是 <c>{image_id, image_url, jump_info}</c> …）。
/// 本类型按 D1 裁剪（<c>.docs/Ads-v3.0-五族端点核验留档-2026-10-11.md</c> §4-D1）：
/// <b>键集建为固定属性、<c>value</c> 以开放字典承载</b> —— 键名守卫锁定、读侧零丢字段；
/// 消费需求驱动时再逐组件建型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeInfo
{
    /// <summary>所属营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>组件化创意 id（官方 <c>dynamic_creative_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("dynamic_creative_id")]
    public long? DynamicCreativeId { get; set; }

    /// <summary>创意名称（官方 <c>dynamic_creative_name</c>）。</summary>
    [JsonPropertyName("dynamic_creative_name")]
    public string? DynamicCreativeName { get; set; }

    /// <summary>创意形式 id（官方 <c>creative_template_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("creative_template_id")]
    public long? CreativeTemplateId { get; set; }

    /// <summary>投放模式（官方 <c>delivery_mode</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("delivery_mode")]
    public string? DeliveryMode { get; set; }

    /// <summary>创意类型（官方 <c>dynamic_creative_type</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("dynamic_creative_type")]
    public string? DynamicCreativeType { get; set; }

    /// <summary>创意组件组（官方 <c>creative_components</c>，44 个组件数组键，见 <see cref="AdsCreativeComponents"/>）。</summary>
    [JsonPropertyName("creative_components")]
    public AdsCreativeComponents? CreativeComponents { get; set; }

    /// <summary>曝光监测链接（官方 <c>impression_tracking_url</c>）。</summary>
    [JsonPropertyName("impression_tracking_url")]
    public string? ImpressionTrackingUrl { get; set; }

    /// <summary>点击监测链接（官方 <c>click_tracking_url</c>）。</summary>
    [JsonPropertyName("click_tracking_url")]
    public string? ClickTrackingUrl { get; set; }

    /// <summary>程序化创意信息（官方 <c>program_creative_info</c>）。</summary>
    [JsonPropertyName("program_creative_info")]
    public AdsProgramCreativeInfo? ProgramCreativeInfo { get; set; }

    /// <summary>落地页监测链接（官方 <c>page_track_url</c>）。</summary>
    [JsonPropertyName("page_track_url")]
    public string? PageTrackUrl { get; set; }

    /// <summary>启用状态（官方 <c>configured_status</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }

    /// <summary>是否已删除（官方 <c>is_deleted</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_deleted")]
    public bool? IsDeleted { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间戳（官方 <c>last_modified_time</c>，秒级）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }

    /// <summary>营销资产审核状态（官方 <c>marketing_asset_verification</c>）。</summary>
    [JsonPropertyName("marketing_asset_verification")]
    public AdsMarketingAssetVerification? MarketingAssetVerification { get; set; }

    /// <summary>创意集审核状态（官方 <c>creative_set_approval_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("creative_set_approval_status")]
    public string? CreativeSetApprovalStatus { get; set; }

    /// <summary>创意来源（官方 <c>source</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>素材不一致状态（官方 <c>asset_inconsistent_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("asset_inconsistent_status")]
    public string? AssetInconsistentStatus { get; set; }

    /// <summary>来源创意 id（官方 <c>source_dynamic_creative_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("source_dynamic_creative_id")]
    public long? SourceDynamicCreativeId { get; set; }

    /// <summary>创意洞察（官方 <c>creative_insight</c>）。</summary>
    [JsonPropertyName("creative_insight")]
    public AdsCreativeInsight? CreativeInsight { get; set; }
}

/// <summary>
/// 创意组件组：44 个官方固定组件键（每个是 <see cref="AdsCreativeComponentItem"/> 数组）。
/// </summary>
/// <remarks>
/// 键集逐字照抄官方（2026-10-11 L3 核验 <c>dynamic_creatives/get</c> 应答表），缺失键在官方应答中缺省
/// ⇒ 全部可空。组件条目的 <c>value</c> 开放形状见 <see cref="AdsCreativeComponentItem"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsCreativeComponents
{
    /// <summary>标题组件（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public List<AdsCreativeComponentItem>? Title { get; set; }

    /// <summary>描述组件（官方键 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public List<AdsCreativeComponentItem>? Description { get; set; }

    /// <summary>图片组件（官方键 <c>image</c>）。</summary>
    [JsonPropertyName("image")]
    public List<AdsCreativeComponentItem>? Image { get; set; }

    /// <summary>图片列表组件（官方键 <c>image_list</c>）。</summary>
    [JsonPropertyName("image_list")]
    public List<AdsCreativeComponentItem>? ImageList { get; set; }

    /// <summary>视频组件（官方键 <c>video</c>）。</summary>
    [JsonPropertyName("video")]
    public List<AdsCreativeComponentItem>? Video { get; set; }

    /// <summary>品牌组件（官方键 <c>brand</c>）。</summary>
    [JsonPropertyName("brand")]
    public List<AdsCreativeComponentItem>? Brand { get; set; }

    /// <summary>咨询组件（官方键 <c>consult</c>）。</summary>
    [JsonPropertyName("consult")]
    public List<AdsCreativeComponentItem>? Consult { get; set; }

    /// <summary>电话组件（官方键 <c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public List<AdsCreativeComponentItem>? Phone { get; set; }

    /// <summary>表单组件（官方键 <c>form</c>）。</summary>
    [JsonPropertyName("form")]
    public List<AdsCreativeComponentItem>? Form { get; set; }

    /// <summary>行动按钮组件（官方键 <c>action_button</c>）。</summary>
    [JsonPropertyName("action_button")]
    public List<AdsCreativeComponentItem>? ActionButton { get; set; }

    /// <summary>双按钮组件（官方键 <c>chosen_button</c>）。</summary>
    [JsonPropertyName("chosen_button")]
    public List<AdsCreativeComponentItem>? ChosenButton { get; set; }

    /// <summary>标签组件（官方键 <c>label</c>）。</summary>
    [JsonPropertyName("label")]
    public List<AdsCreativeComponentItem>? Label { get; set; }

    /// <summary>数据展示组件（官方键 <c>show_data</c>）。</summary>
    [JsonPropertyName("show_data")]
    public List<AdsCreativeComponentItem>? ShowData { get; set; }

    /// <summary>营销挂件组件（官方键 <c>marketing_pendant</c>）。</summary>
    [JsonPropertyName("marketing_pendant")]
    public List<AdsCreativeComponentItem>? MarketingPendant { get; set; }

    /// <summary>App 礼包码组件（官方键 <c>app_gift_pack_code</c>）。</summary>
    [JsonPropertyName("app_gift_pack_code")]
    public List<AdsCreativeComponentItem>? AppGiftPackCode { get; set; }

    /// <summary>商店图组件（官方键 <c>shop_image</c>）。</summary>
    [JsonPropertyName("shop_image")]
    public List<AdsCreativeComponentItem>? ShopImage { get; set; }

    /// <summary>倒计时组件（官方键 <c>count_down</c>）。</summary>
    [JsonPropertyName("count_down")]
    public List<AdsCreativeComponentItem>? CountDown { get; set; }

    /// <summary>弹幕组件（官方键 <c>barrage</c>）。</summary>
    [JsonPropertyName("barrage")]
    public List<AdsCreativeComponentItem>? Barrage { get; set; }

    /// <summary>悬浮窗组件（官方键 <c>floating_zone</c>）。</summary>
    [JsonPropertyName("floating_zone")]
    public List<AdsCreativeComponentItem>? FloatingZone { get; set; }

    /// <summary>文本链组件（官方键 <c>text_link</c>）。</summary>
    [JsonPropertyName("text_link")]
    public List<AdsCreativeComponentItem>? TextLink { get; set; }

    /// <summary>结束页组件（官方键 <c>end_page</c>）。</summary>
    [JsonPropertyName("end_page")]
    public List<AdsCreativeComponentItem>? EndPage { get; set; }

    /// <summary>直播描述组件（官方键 <c>living_desc</c>）。</summary>
    [JsonPropertyName("living_desc")]
    public List<AdsCreativeComponentItem>? LivingDesc { get; set; }

    /// <summary>视频号组件（官方键 <c>wechat_channels</c>）。</summary>
    [JsonPropertyName("wechat_channels")]
    public List<AdsCreativeComponentItem>? WechatChannels { get; set; }

    /// <summary>短视频组件（官方键 <c>short_video</c>）。</summary>
    [JsonPropertyName("short_video")]
    public List<AdsCreativeComponentItem>? ShortVideo { get; set; }

    /// <summary>故事化组件（官方键 <c>element_story</c>）。</summary>
    [JsonPropertyName("element_story")]
    public List<AdsCreativeComponentItem>? ElementStory { get; set; }

    /// <summary>微信小游戏试玩组件（官方键 <c>wxgame_playable_page</c>）。</summary>
    [JsonPropertyName("wxgame_playable_page")]
    public List<AdsCreativeComponentItem>? WxgamePlayablePage { get; set; }

    /// <summary>主跳转组件（官方键 <c>main_jump_info</c>）。</summary>
    [JsonPropertyName("main_jump_info")]
    public List<AdsCreativeComponentItem>? MainJumpInfo { get; set; }

    /// <summary>App 推广视频组件（官方键 <c>app_promotion_video</c>）。</summary>
    [JsonPropertyName("app_promotion_video")]
    public List<AdsCreativeComponentItem>? AppPromotionVideo { get; set; }

    /// <summary>视频橱窗组件（官方键 <c>video_showcase</c>）。</summary>
    [JsonPropertyName("video_showcase")]
    public List<AdsCreativeComponentItem>? VideoShowcase { get; set; }

    /// <summary>图片橱窗组件（官方键 <c>image_showcase</c>）。</summary>
    [JsonPropertyName("image_showcase")]
    public List<AdsCreativeComponentItem>? ImageShowcase { get; set; }

    /// <summary>社交技能组件（官方键 <c>social_skill</c>）。</summary>
    [JsonPropertyName("social_skill")]
    public List<AdsCreativeComponentItem>? SocialSkill { get; set; }

    /// <summary>迷你卡片链组件（官方键 <c>mini_card_link</c>）。</summary>
    [JsonPropertyName("mini_card_link")]
    public List<AdsCreativeComponentItem>? MiniCardLink { get; set; }

    /// <summary>悬浮窗列表组件（官方键 <c>floating_zone_list</c>）。</summary>
    [JsonPropertyName("floating_zone_list")]
    public List<AdsCreativeComponentItem>? FloatingZoneList { get; set; }

    /// <summary>视频号内容组件（官方键 <c>video_channels_content</c>）。</summary>
    [JsonPropertyName("video_channels_content")]
    public List<AdsCreativeComponentItem>? VideoChannelsContent { get; set; }

    /// <summary>微信小店活动组件（官方键 <c>wechat_shop_activity</c>）。</summary>
    [JsonPropertyName("wechat_shop_activity")]
    public List<AdsCreativeComponentItem>? WechatShopActivity { get; set; }

    /// <summary>音频组件（官方键 <c>audio</c>）。</summary>
    [JsonPropertyName("audio")]
    public List<AdsCreativeComponentItem>? Audio { get; set; }

    /// <summary>小游戏直跳组件（官方键 <c>wxgame_direct_page</c>）。</summary>
    [JsonPropertyName("wxgame_direct_page")]
    public List<AdsCreativeComponentItem>? WxgameDirectPage { get; set; }

    /// <summary>视频列表组件（官方键 <c>video_list</c>）。</summary>
    [JsonPropertyName("video_list")]
    public List<AdsCreativeComponentItem>? VideoList { get; set; }

    /// <summary>医生名片组件（官方键 <c>doctor_card</c>）。</summary>
    [JsonPropertyName("doctor_card")]
    public List<AdsCreativeComponentItem>? DoctorCard { get; set; }

    /// <summary>视频号直播组件（官方键 <c>channels_live_feed</c>）。</summary>
    [JsonPropertyName("channels_live_feed")]
    public List<AdsCreativeComponentItem>? ChannelsLiveFeed { get; set; }

    /// <summary>激励浏览组件（官方键 <c>rewarded_browse</c>）。</summary>
    [JsonPropertyName("rewarded_browse")]
    public List<AdsCreativeComponentItem>? RewardedBrowse { get; set; }

    /// <summary>视频号品牌组件（官方键 <c>channels_brand</c>）。</summary>
    [JsonPropertyName("channels_brand")]
    public List<AdsCreativeComponentItem>? ChannelsBrand { get; set; }

    /// <summary>MDPA 标题组件（官方键 <c>mdpa_title</c>）。</summary>
    [JsonPropertyName("mdpa_title")]
    public List<AdsCreativeComponentItem>? MdpaTitle { get; set; }

    /// <summary>MDPA 描述组件（官方键 <c>mdpa_description</c>）。</summary>
    [JsonPropertyName("mdpa_description")]
    public List<AdsCreativeComponentItem>? MdpaDescription { get; set; }
}

/// <summary>创意组件条目（官方 <c>{component_id, value, is_deleted}</c>，两种容器形态共用：数组键与单数键）。</summary>
/// <remarks>
/// <para>
/// <b><c>value</c> 是逐组件 union</b>（D1 裁剪）：一期以 <see cref="Dictionary{TKey, TValue}"/> 承载 ——
/// 官方键原名入袋（字典键不经命名策略转换，与报表行先例同源），读侧零丢字段。
/// <b>写侧</b>（<c>dynamic_creatives/add|update</c>、<c>components/add</c>）由调用方以 JSON 字面量构造，
/// 例：<c>JsonSerializer.Deserialize&lt;Dictionary&lt;string, JsonElement&gt;&gt;("""{"content":"标题"}""", context)</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsCreativeComponentItem
{
    /// <summary>组件引用 id（官方 <c>component_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("component_id")]
    public long? ComponentId { get; set; }

    /// <summary>组件值（官方 <c>value</c>，逐组件 union，开放字典承载 —— 见类型 remarks）。</summary>
    [JsonPropertyName("value")]
    public Dictionary<string, JsonElement>? Value { get; set; }

    /// <summary>是否已删除（官方 <c>is_deleted</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_deleted")]
    public bool? IsDeleted { get; set; }
}

/// <summary>程序化创意信息（官方 <c>program_creative_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsProgramCreativeInfo
{
    /// <summary>素材衍生任务 id（官方 <c>material_derive_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("material_derive_id")]
    public long? MaterialDeriveId { get; set; }

    /// <summary>素材衍生信息列表（官方 <c>material_derive_info</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("material_derive_info")]
    public List<AdsMaterialDeriveInfo>? MaterialDeriveInfo { get; set; }

    /// <summary>出价模式（官方 <c>bid_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("bid_mode")]
    public string? BidMode { get; set; }

    /// <summary>衍生版本（官方 <c>derive_version</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("derive_version")]
    public string? DeriveVersion { get; set; }
}

/// <summary>素材衍生信息（官方 <c>material_derive_info</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsMaterialDeriveInfo
{
    /// <summary>原素材 id 列表（官方 <c>original_material_id_list</c>，<c>string[]</c>）。</summary>
    [JsonPropertyName("original_material_id_list")]
    public List<string>? OriginalMaterialIdList { get; set; }

    /// <summary>原创意形式 id 列表（官方 <c>original_adcreative_template_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("original_adcreative_template_id_list")]
    public List<long>? OriginalAdcreativeTemplateIdList { get; set; }

    /// <summary>原封面图 id（官方 <c>original_cover_image_id</c>）。</summary>
    [JsonPropertyName("original_cover_image_id")]
    public string? OriginalCoverImageId { get; set; }

    /// <summary>衍生数据列表（官方 <c>derive_data_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("derive_data_list")]
    public List<AdsMaterialDeriveData>? DeriveDataList { get; set; }

    /// <summary>原组件 id（官方 <c>original_component_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("original_component_id")]
    public long? OriginalComponentId { get; set; }
}

/// <summary>素材衍生数据（官方 <c>derive_data_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsMaterialDeriveData
{
    /// <summary>衍生创意形式 id（官方 <c>derive_template_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("derive_template_id")]
    public long? DeriveTemplateId { get; set; }

    /// <summary>衍生创意形式 id 列表（官方 <c>derive_adcreative_template_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("derive_adcreative_template_id_list")]
    public List<long>? DeriveAdcreativeTemplateIdList { get; set; }

    /// <summary>创意元素使用配置（官方 <c>creative_elements_usage</c>）。</summary>
    [JsonPropertyName("creative_elements_usage")]
    public AdsCreativeElementsUsage? CreativeElementsUsage { get; set; }

    /// <summary>素材衍生预览 id（官方 <c>material_derive_preview_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("material_derive_preview_id")]
    public long? MaterialDerivePreviewId { get; set; }
}

/// <summary>创意元素使用配置（官方 <c>creative_elements_usage</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsCreativeElementsUsage
{
    /// <summary>是否使用描述元素（官方 <c>use_description_element</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("use_description_element")]
    public bool? UseDescriptionElement { get; set; }
}

/// <summary>营销资产审核状态（官方 <c>marketing_asset_verification</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsMarketingAssetVerification
{
    /// <summary>审核状态（官方 <c>marketing_asset_verification_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_asset_verification_status")]
    public string? Status { get; set; }

    /// <summary>审核状态中文描述（官方 <c>marketing_asset_verification_status_cn</c>）。</summary>
    [JsonPropertyName("marketing_asset_verification_status_cn")]
    public string? StatusCn { get; set; }

    /// <summary>落地页审核列表（官方 <c>landing_page_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("landing_page_list")]
    public List<AdsMarketingAssetLandingPage>? LandingPageList { get; set; }
}

/// <summary>营销资产落地页审核条目（官方 <c>landing_page_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsMarketingAssetLandingPage
{
    /// <summary>落地页名称（官方 <c>landing_page_name</c>）。</summary>
    [JsonPropertyName("landing_page_name")]
    public string? LandingPageName { get; set; }

    /// <summary>审核状态（官方 <c>marketing_asset_verification_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_asset_verification_status")]
    public string? Status { get; set; }

    /// <summary>审核状态中文描述（官方 <c>marketing_asset_verification_status_cn</c>）。</summary>
    [JsonPropertyName("marketing_asset_verification_status_cn")]
    public string? StatusCn { get; set; }

    /// <summary>跳转信息（官方 <c>jump_info</c>，含 <c>page_type</c> 与 20+ spec union —— D2 开放承载）。</summary>
    [JsonPropertyName("jump_info")]
    public Dictionary<string, JsonElement>? JumpInfo { get; set; }
}

/// <summary>创意洞察（官方 <c>creative_insight</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsCreativeInsight
{
    /// <summary>重复组件 id 列表（官方 <c>duplicate_component_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("duplicate_component_id_list")]
    public List<long>? DuplicateComponentIdList { get; set; }
}

/// <summary>
/// <c>dynamic_creatives/get</c> 的游标分页元信息（官方 <c>cursor_page_info</c>，D6 逐域分建）。
/// </summary>
/// <remarks>
/// 与 <c>advertiser/get</c> 的 <c>cursor_page_info</c>（<c>{page_size, total_number, has_more, cursor}</c>，
/// <c>cursor</c> 为 integer）不同构：本页游标是 <c>string</c>（<c>next_cursor</c> / <c>previous_cursor</c>）
/// 且无 <c>has_more</c> —— 收敛会把某一页的字段名当成全页事实，故独立建型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeCursorPageInfo
{
    /// <summary>每页条数（官方 <c>page_size</c>）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>总条数（官方 <c>total_number</c>）。</summary>
    [JsonPropertyName("total_number")]
    public long? TotalNumber { get; set; }

    /// <summary>下一页游标（官方 <c>next_cursor</c>，<c>string</c>）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>上一页游标（官方 <c>previous_cursor</c>，<c>string</c>）。</summary>
    [JsonPropertyName("previous_cursor")]
    public string? PreviousCursor { get; set; }
}

/// <summary><c>dynamic_creatives/get</c> 的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeListData
{
    /// <summary>创意列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsDynamicCreativeInfo>? List { get; set; }

    /// <summary>普通分页元信息（官方 <c>page_info</c>；<c>pagination_mode = NORMAL</c> 时返回）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }

    /// <summary>游标分页元信息（官方 <c>cursor_page_info</c>；<c>pagination_mode = CURSOR</c> 时返回）。</summary>
    [JsonPropertyName("cursor_page_info")]
    public AdsDynamicCreativeCursorPageInfo? CursorPageInfo { get; set; }
}

/// <summary><c>GET /v3.0/dynamic_creatives/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "DynamicCreatives")]
public class AdsDynamicCreativeGetResponse : AdsResponse<AdsDynamicCreativeListData>
{
}
