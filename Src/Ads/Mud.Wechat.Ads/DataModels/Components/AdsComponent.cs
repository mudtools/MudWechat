// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.DynamicCreatives;

namespace Mud.Wechat.Ads.DataModels.Components;

/// <summary>
/// 创意组件值（官方 <c>component_value</c>，<c>components/get</c> 应答与 <c>components/add</c> 请求共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>单数形态</b>：与 <c>dynamic_creatives</c> 的 <c>creative_components</c>（44 个<b>数组</b>键）不同，
/// 本类型的 40 个组件键是<b>单个</b> <c>struct</c>（元素同构 <c>{component_id, value, is_deleted}</c>）。
/// 两份键集同名但不等（本键集多 <c>jump_info</c>、少 <c>main_jump_info</c> / <c>mdpa_title</c> /
/// <c>mdpa_description</c> / <c>audio</c> 等 —— 2026-10-11 L3 逐页核验），<b>不得</b>合并成一个类型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentValue
{
    /// <summary>标题组件（官方键 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public AdsCreativeComponentItem? Title { get; set; }

    /// <summary>描述组件（官方键 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public AdsCreativeComponentItem? Description { get; set; }

    /// <summary>图片组件（官方键 <c>image</c>）。</summary>
    [JsonPropertyName("image")]
    public AdsCreativeComponentItem? Image { get; set; }

    /// <summary>图片列表组件（官方键 <c>image_list</c>）。</summary>
    [JsonPropertyName("image_list")]
    public AdsCreativeComponentItem? ImageList { get; set; }

    /// <summary>视频组件（官方键 <c>video</c>）。</summary>
    [JsonPropertyName("video")]
    public AdsCreativeComponentItem? Video { get; set; }

    /// <summary>品牌组件（官方键 <c>brand</c>）。</summary>
    [JsonPropertyName("brand")]
    public AdsCreativeComponentItem? Brand { get; set; }

    /// <summary>咨询组件（官方键 <c>consult</c>）。</summary>
    [JsonPropertyName("consult")]
    public AdsCreativeComponentItem? Consult { get; set; }

    /// <summary>电话组件（官方键 <c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public AdsCreativeComponentItem? Phone { get; set; }

    /// <summary>表单组件（官方键 <c>form</c>）。</summary>
    [JsonPropertyName("form")]
    public AdsCreativeComponentItem? Form { get; set; }

    /// <summary>行动按钮组件（官方键 <c>action_button</c>）。</summary>
    [JsonPropertyName("action_button")]
    public AdsCreativeComponentItem? ActionButton { get; set; }

    /// <summary>双按钮组件（官方键 <c>chosen_button</c>）。</summary>
    [JsonPropertyName("chosen_button")]
    public AdsCreativeComponentItem? ChosenButton { get; set; }

    /// <summary>标签组件（官方键 <c>label</c>）。</summary>
    [JsonPropertyName("label")]
    public AdsCreativeComponentItem? Label { get; set; }

    /// <summary>数据展示组件（官方键 <c>show_data</c>）。</summary>
    [JsonPropertyName("show_data")]
    public AdsCreativeComponentItem? ShowData { get; set; }

    /// <summary>营销挂件组件（官方键 <c>marketing_pendant</c>）。</summary>
    [JsonPropertyName("marketing_pendant")]
    public AdsCreativeComponentItem? MarketingPendant { get; set; }

    /// <summary>App 礼包码组件（官方键 <c>app_gift_pack_code</c>）。</summary>
    [JsonPropertyName("app_gift_pack_code")]
    public AdsCreativeComponentItem? AppGiftPackCode { get; set; }

    /// <summary>商店图组件（官方键 <c>shop_image</c>）。</summary>
    [JsonPropertyName("shop_image")]
    public AdsCreativeComponentItem? ShopImage { get; set; }

    /// <summary>倒计时组件（官方键 <c>count_down</c>）。</summary>
    [JsonPropertyName("count_down")]
    public AdsCreativeComponentItem? CountDown { get; set; }

    /// <summary>弹幕组件（官方键 <c>barrage</c>）。</summary>
    [JsonPropertyName("barrage")]
    public AdsCreativeComponentItem? Barrage { get; set; }

    /// <summary>悬浮窗组件（官方键 <c>floating_zone</c>）。</summary>
    [JsonPropertyName("floating_zone")]
    public AdsCreativeComponentItem? FloatingZone { get; set; }

    /// <summary>文本链组件（官方键 <c>text_link</c>）。</summary>
    [JsonPropertyName("text_link")]
    public AdsCreativeComponentItem? TextLink { get; set; }

    /// <summary>跳转组件（官方键 <c>jump_info</c>；仅本域有）。</summary>
    [JsonPropertyName("jump_info")]
    public AdsCreativeComponentItem? JumpInfo { get; set; }

    /// <summary>结束页组件（官方键 <c>end_page</c>）。</summary>
    [JsonPropertyName("end_page")]
    public AdsCreativeComponentItem? EndPage { get; set; }

    /// <summary>直播描述组件（官方键 <c>living_desc</c>）。</summary>
    [JsonPropertyName("living_desc")]
    public AdsCreativeComponentItem? LivingDesc { get; set; }

    /// <summary>视频号组件（官方键 <c>wechat_channels</c>）。</summary>
    [JsonPropertyName("wechat_channels")]
    public AdsCreativeComponentItem? WechatChannels { get; set; }

    /// <summary>短视频组件（官方键 <c>short_video</c>）。</summary>
    [JsonPropertyName("short_video")]
    public AdsCreativeComponentItem? ShortVideo { get; set; }

    /// <summary>故事化组件（官方键 <c>element_story</c>）。</summary>
    [JsonPropertyName("element_story")]
    public AdsCreativeComponentItem? ElementStory { get; set; }

    /// <summary>微信小游戏试玩组件（官方键 <c>wxgame_playable_page</c>）。</summary>
    [JsonPropertyName("wxgame_playable_page")]
    public AdsCreativeComponentItem? WxgamePlayablePage { get; set; }

    /// <summary>App 推广视频组件（官方键 <c>app_promotion_video</c>）。</summary>
    [JsonPropertyName("app_promotion_video")]
    public AdsCreativeComponentItem? AppPromotionVideo { get; set; }

    /// <summary>视频橱窗组件（官方键 <c>video_showcase</c>）。</summary>
    [JsonPropertyName("video_showcase")]
    public AdsCreativeComponentItem? VideoShowcase { get; set; }

    /// <summary>图片橱窗组件（官方键 <c>image_showcase</c>）。</summary>
    [JsonPropertyName("image_showcase")]
    public AdsCreativeComponentItem? ImageShowcase { get; set; }

    /// <summary>社交技能组件（官方键 <c>social_skill</c>）。</summary>
    [JsonPropertyName("social_skill")]
    public AdsCreativeComponentItem? SocialSkill { get; set; }

    /// <summary>迷你卡片链组件（官方键 <c>mini_card_link</c>）。</summary>
    [JsonPropertyName("mini_card_link")]
    public AdsCreativeComponentItem? MiniCardLink { get; set; }

    /// <summary>悬浮窗列表组件（官方键 <c>floating_zone_list</c>）。</summary>
    [JsonPropertyName("floating_zone_list")]
    public AdsCreativeComponentItem? FloatingZoneList { get; set; }

    /// <summary>视频号内容组件（官方键 <c>video_channels_content</c>）。</summary>
    [JsonPropertyName("video_channels_content")]
    public AdsCreativeComponentItem? VideoChannelsContent { get; set; }

    /// <summary>微信小店活动组件（官方键 <c>wechat_shop_activity</c>）。</summary>
    [JsonPropertyName("wechat_shop_activity")]
    public AdsCreativeComponentItem? WechatShopActivity { get; set; }

    /// <summary>小游戏直跳组件（官方键 <c>wxgame_direct_page</c>）。</summary>
    [JsonPropertyName("wxgame_direct_page")]
    public AdsCreativeComponentItem? WxgameDirectPage { get; set; }

    /// <summary>视频列表组件（官方键 <c>video_list</c>）。</summary>
    [JsonPropertyName("video_list")]
    public AdsCreativeComponentItem? VideoList { get; set; }

    /// <summary>视频号直播组件（官方键 <c>channels_live_feed</c>）。</summary>
    [JsonPropertyName("channels_live_feed")]
    public AdsCreativeComponentItem? ChannelsLiveFeed { get; set; }

    /// <summary>医生名片组件（官方键 <c>doctor_card</c>）。</summary>
    [JsonPropertyName("doctor_card")]
    public AdsCreativeComponentItem? DoctorCard { get; set; }

    /// <summary>激励浏览组件（官方键 <c>rewarded_browse</c>）。</summary>
    [JsonPropertyName("rewarded_browse")]
    public AdsCreativeComponentItem? RewardedBrowse { get; set; }
}

/// <summary>创意组件条目（官方 <c>components/get</c> 的 <c>data.list[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentInfo
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>组件 id（官方 <c>component_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("component_id")]
    public long? ComponentId { get; set; }

    /// <summary>组件值（官方 <c>component_value</c>，40 组件单数键，见 <see cref="AdsComponentValue"/>）。</summary>
    [JsonPropertyName("component_value")]
    public AdsComponentValue? ComponentValue { get; set; }

    /// <summary>创建时间戳（官方 <c>created_time</c>，秒级）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间戳（官方 <c>last_modified_time</c>，秒级）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }

    /// <summary>组件子类型（官方 <c>component_sub_type</c>，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("component_sub_type")]
    public string? ComponentSubType { get; set; }

    /// <summary>自定义名称（官方 <c>component_custom_name</c>）。</summary>
    [JsonPropertyName("component_custom_name")]
    public string? ComponentCustomName { get; set; }

    /// <summary>生成方式（官方 <c>generation_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("generation_type")]
    public string? GenerationType { get; set; }

    /// <summary>是否已删除（官方 <c>is_deleted</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_deleted")]
    public bool? IsDeleted { get; set; }

    /// <summary>相似状态（官方 <c>similarity_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("similarity_status")]
    public string? SimilarityStatus { get; set; }

    /// <summary>潜在状态（官方 <c>potential_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("potential_status")]
    public string? PotentialStatus { get; set; }

    /// <summary>禁用原因（官方 <c>disable_message</c>）。</summary>
    [JsonPropertyName("disable_message")]
    public string? DisableMessage { get; set; }

    /// <summary>首发状态（官方 <c>first_publication_status</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("first_publication_status")]
    public string? FirstPublicationStatus { get; set; }

    /// <summary>使用场景（官方 <c>scene</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }
}

/// <summary><c>components/get</c> 的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentListData
{
    /// <summary>组件列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsComponentInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/components/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentGetResponse : AdsResponse<AdsComponentListData>
{
}

/// <summary>
/// <c>POST /v3.0/components/add</c> 的请求体（2026-10-11 L3 核验，5 键）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>自定义名称（官方 <c>component_custom_name</c>）。</summary>
    [JsonPropertyName("component_custom_name")]
    public string? ComponentCustomName { get; set; }

    /// <summary>组件值（官方 <c>component_value</c>，必填；40 组件单数键见 <see cref="AdsComponentValue"/>）。</summary>
    [JsonPropertyName("component_value")]
    public AdsComponentValue? ComponentValue { get; set; }

    /// <summary>组件子类型（官方 <c>component_sub_type</c>，必填，<c>enum</c>；可选值未逐项核验）。</summary>
    [JsonPropertyName("component_sub_type")]
    public string? ComponentSubType { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }
}

/// <summary>
/// <c>POST /v3.0/components/delete</c> 的请求体（4 键）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentDeleteRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组件 id（官方 <c>component_id</c>，必填）。</summary>
    [JsonPropertyName("component_id")]
    public long? ComponentId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>删除策略（官方 <c>delete_strategy</c>，<c>enum</c>；可选值
    /// <c>{DELETE_STRATEGY_FORCE, DELETE_STRATEGY_RESTRICTED}</c> —— 2026-10-11 枚举详情核验）。</summary>
    [JsonPropertyName("delete_strategy")]
    public string? DeleteStrategy { get; set; }
}

/// <summary><c>components/add|delete</c> 共用的 <c>data</c> 载荷（两支应答同构，均只有组件 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentIdData
{
    /// <summary>组件 id（官方 <c>component_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("component_id")]
    public long? ComponentId { get; set; }
}

/// <summary><c>POST /v3.0/components/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentAddResponse : AdsResponse<AdsComponentIdData>
{
}

/// <summary><c>POST /v3.0/components/delete</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentDeleteResponse : AdsResponse<AdsComponentIdData>
{
}
