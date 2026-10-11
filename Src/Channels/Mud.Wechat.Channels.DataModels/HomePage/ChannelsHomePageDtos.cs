// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 获取主页展示商品列表（<c>getstorewindowproductlist</c>）请求体。
/// </summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；本域为小店店铺主页（<c>/channels/ec/store/window/*</c>），非视频号橱窗。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetStoreWindowProductListRequest
{
    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，默认 10，不超过 30）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，选填；由上次请求返回，不传默认获取第一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>
/// 获取主页展示商品列表（<c>getstorewindowproductlist</c>）响应。
/// </summary>
/// <remarks>官方契约：仅返回<b>上架中</b>的商品。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetStoreWindowProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置主页展示商品列表（官方 <c>products</c>）。</summary>
    [JsonPropertyName("products")]
    public List<ChannelsStoreWindowProduct>? Products { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>next_key</c>，用于请求下一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>主页展示商品信息（官方 <c>products</c> 数组元素，<c>getstorewindowproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsStoreWindowProduct
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置是否隐藏（官方 <c>is_set_hide</c>；1=隐藏，0=取消隐藏。隐藏的商品只在首页不可见，并不代表下架）。</summary>
    [JsonPropertyName("is_set_hide")]
    public int? IsSetHide { get; set; }

    /// <summary>获取或设置是否置顶（官方 <c>is_set_top</c>；1=置顶，0=取消置顶）。</summary>
    [JsonPropertyName("is_set_top")]
    public int? IsSetTop { get; set; }
}

/// <summary>重新排序主页展示商品（<c>reorderstorewindowproduct</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsReorderStoreWindowProductRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品重新排序后的新序号（官方 <c>index_num</c>，必填，最大移动步长为 500）。</summary>
    [JsonPropertyName("index_num")]
    public int IndexNum { get; set; }
}

/// <summary>隐藏小店主页商品（<c>hidestorewindowproduct</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsHideStoreWindowProductRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置是否隐藏（官方 <c>is_set_hide</c>，必填；1=隐藏，0=取消隐藏。隐藏不会下架，仍可通过其他渠道购买）。</summary>
    [JsonPropertyName("is_set_hide")]
    public int IsSetHide { get; set; }
}

/// <summary>置顶小店主页商品（<c>settopstorewindowproduct</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsSetTopStoreWindowProductRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置是否置顶（官方 <c>is_set_top</c>，必填；1=置顶，0=取消置顶）。</summary>
    [JsonPropertyName("is_set_top")]
    public int IsSetTop { get; set; }
}

/// <summary>
/// 提交主页背景图申请（<c>submitbackgroundapply</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：图片支持 bmp、jpg/jpeg、png、svg、webp 格式，上传大小上限 10MB；
/// 须用「上传图片」接口（resp_type=1）返回的 img_url，不接受其他格式的图片 url（前缀 <c>mmecimage.cn/p/</c> 可直接提交）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsSubmitHomepageBackgroundApplyRequest
{
    /// <summary>获取或设置图片链接（官方 <c>img_url</c>，必填；经「上传图片」接口获取，仅支持一张背景图，按 16:9 比例展示在主页顶部）。</summary>
    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; } = string.Empty;
}

/// <summary>
/// 查询背景图（<c>getbackground</c>）响应。
/// </summary>
/// <remarks>官方契约：申请对象仅返回最近一次<b>流程中</b>的申请；已生效/已撤销的申请不返回。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetHomepageBackgroundResponse : ChannelsResponse
{
    /// <summary>获取或设置当前生效的背景图片 url（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>获取或设置最近一次流程中的申请（官方 <c>apply</c>；已生效/已撤销的申请不返回）。</summary>
    [JsonPropertyName("apply")]
    public ChannelsHomepageBackgroundApply? Apply { get; set; }
}

/// <summary>主页背景图申请信息（官方 <c>apply</c> 对象，<c>getbackground</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsHomepageBackgroundApply
{
    /// <summary>获取或设置申请编号（官方 <c>apply_id</c>）。</summary>
    [JsonPropertyName("apply_id")]
    public long? ApplyId { get; set; }

    /// <summary>获取或设置申请状态（官方 <c>state</c>，见 <see cref="ChannelsHomepageApplyStates"/>）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置审核结果描述（官方 <c>audit_desc</c>；state 为审核驳回时有值）。</summary>
    [JsonPropertyName("audit_desc")]
    public string? AuditDesc { get; set; }

    /// <summary>获取或设置图片 url（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}

/// <summary>
/// 撤销申请请求体（背景图 <c>background/apply/cancel</c> 与精选展示位 <c>banner/apply/cancel</c> 请求体字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsHomepageApplyIdRequest
{
    /// <summary>获取或设置申请编号（官方 <c>apply_id</c>，必填；仅能撤销流程中的申请）。</summary>
    [JsonPropertyName("apply_id")]
    public long ApplyId { get; set; }
}

/// <summary>
/// 提交申请响应（背景图 <c>background/apply/submit</c> 与精选展示位 <c>banner/apply/submit</c> 响应字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsHomepageApplyResponse : ChannelsResponse
{
    /// <summary>获取或设置申请编号（官方 <c>apply_id</c>）。</summary>
    [JsonPropertyName("apply_id")]
    public long? ApplyId { get; set; }
}

/// <summary>
/// 提交精选展示位申请（<c>submitbannerapply</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：请求体为嵌套形态 <c>{"banner":{…}}</c>（官方参数表与请求示例原文，照抄建模）；
/// 展示位最多 5 个，小图模式（scale=1）最少 3 个；模块标题最多 32 字。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsSubmitBannerApplyRequest
{
    /// <summary>获取或设置精选展示位模块（官方 <c>banner</c>，必填）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerModule Banner { get; set; } = new();
}

/// <summary>
/// 精选展示位模块（官方 <c>banner</c> 对象；提交请求体与 <c>getbanner</c> 响应的生效模块/申请模块共用——
/// 查询响应不携带 <c>title</c>，提交时必填 <c>banner</c> 数组与 <c>scale</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerModule
{
    /// <summary>获取或设置展示位列表（官方 <c>banner</c>；提交时必填，最多 5 个，小图模式最少 3 个）。</summary>
    [JsonPropertyName("banner")]
    public List<ChannelsBannerItem>? Banner { get; set; }

    /// <summary>获取或设置展示位的展示样式（官方 <c>scale</c>，见 <see cref="ChannelsBannerScales"/>；提交时必填）。</summary>
    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    /// <summary>获取或设置模块标题（官方 <c>title</c>，选填，最多 32 字）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

/// <summary>
/// 精选展示位项（官方 <c>banner.banner[]</c> 数组元素；提交请求体、<c>getbanner</c> 生效列表与申请明细共用——三处结构一致）。
/// </summary>
/// <remarks>
/// 官方契约：<b>type=商品/视频号/公众号三选一，只能设置对应类型的参数</b>（官方错误码 268552621/268552622/268553233）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerItem
{
    /// <summary>获取或设置展示位内容（官方 <c>banner</c>：图片 / 标题 / 描述；提交时必填）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerContent? Banner { get; set; }

    /// <summary>获取或设置展示位类型（官方 <c>type</c>，见 <see cref="ChannelsBannerTypes"/>；提交时必填）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置商品参数（官方 <c>product</c>；type=1 商品时设置）。</summary>
    [JsonPropertyName("product")]
    public ChannelsBannerProduct? Product { get; set; }

    /// <summary>获取或设置视频号参数（官方 <c>finder</c>；type=3 视频号时设置）。</summary>
    [JsonPropertyName("finder")]
    public ChannelsBannerFinder? Finder { get; set; }

    /// <summary>获取或设置公众号参数（官方 <c>official_account</c>；type=4 公众号时设置）。</summary>
    [JsonPropertyName("official_account")]
    public ChannelsBannerOfficialAccount? OfficialAccount { get; set; }
}

/// <summary>精选展示位内容（官方 <c>banner.banner[].banner</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerContent
{
    /// <summary>获取或设置图片 url（官方 <c>img_url</c>；提交时必填。须用「上传图片」接口 resp_type=1 上传，不接受其他任何格式的图片 url）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>获取或设置展示位标题（官方 <c>title</c>；提交时必填。scale=1 时最多 12 字，scale=2 时最多 40 字）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置描述（官方 <c>description</c>；scale=2 时可设，最多 100 字；scale=1 时不可设置）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>精选展示位商品参数（官方 <c>banner.banner[].product</c> 对象，type=1 时设置）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerProduct
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>；可通过「获取商品列表」接口查询，商品必须为上架状态）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }
}

/// <summary>精选展示位视频号参数（官方 <c>banner.banner[].finder</c> 对象，type=3 时设置）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerFinder
{
    /// <summary>获取或设置视频号 ID（官方 <c>finder_user_name</c>；登录视频号助手在首页查看）。</summary>
    [JsonPropertyName("finder_user_name")]
    public string? FinderUserName { get; set; }

    /// <summary>获取或设置视频 ID（官方 <c>feed_id</c>，视频号视频的唯一标识；登录视频号助手在「动态管理」模块复制）。</summary>
    [JsonPropertyName("feed_id")]
    public string? FeedId { get; set; }
}

/// <summary>精选展示位公众号参数（官方 <c>banner.banner[].official_account</c> 对象，type=4 时设置）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerOfficialAccount
{
    /// <summary>获取或设置公众号文章链接（官方 <c>url</c>；可在微信客户端打开公众号文章获取分享链接）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 查询精选展示位（<c>getbanner</c>）响应。
/// </summary>
/// <remarks>官方契约：申请对象仅返回最近一次<b>流程中</b>的申请；不返回已生效或已撤销的申请。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetBannerResponse : ChannelsResponse
{
    /// <summary>获取或设置当前生效的展示位（官方 <c>banner</c>）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerModule? Banner { get; set; }

    /// <summary>获取或设置最近一次流程中的申请（官方 <c>apply</c>；不返回已生效或已撤销的申请）。</summary>
    [JsonPropertyName("apply")]
    public ChannelsBannerApply? Apply { get; set; }
}

/// <summary>精选展示位申请信息（官方 <c>apply</c> 对象，<c>getbanner</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerApply
{
    /// <summary>获取或设置申请编号（官方 <c>apply_id</c>）。</summary>
    [JsonPropertyName("apply_id")]
    public long? ApplyId { get; set; }

    /// <summary>获取或设置申请状态（官方 <c>state</c>，见 <see cref="ChannelsHomepageApplyStates"/>）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置展示位的展示样式（官方 <c>scale</c>，见 <see cref="ChannelsBannerScales"/>）。</summary>
    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    /// <summary>获取或设置精选展示位申请明细（官方 <c>banner</c>）。</summary>
    [JsonPropertyName("banner")]
    public List<ChannelsBannerApplyItem>? Banner { get; set; }
}

/// <summary>精选展示位申请明细（官方 <c>apply.banner[]</c> 数组元素，<c>getbanner</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerApplyItem
{
    /// <summary>获取或设置展示位（官方 <c>banner</c>，结构与生效展示位一致）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerItem? Banner { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>audit_state</c>，见 <see cref="ChannelsHomepageApplyStates"/>）。</summary>
    [JsonPropertyName("audit_state")]
    public int? AuditState { get; set; }

    /// <summary>获取或设置审核结果描述（官方 <c>audit_desc</c>；audit_state 为驳回时有值）。</summary>
    [JsonPropertyName("audit_desc")]
    public string? AuditDesc { get; set; }
}

/// <summary>
/// 获取在店铺主页展示的商品分类（<c>getclassificationtree</c>）响应。
/// </summary>
/// <remarks>官方契约：仅分类树，不含分类与商品的关联数据（关联商品走 <c>tree/product/get</c>）；支持 2 级树结构。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetClassificationTreeResponse : ChannelsResponse
{
    /// <summary>获取或设置分类树（官方 <c>tree</c>）。</summary>
    [JsonPropertyName("tree")]
    public ChannelsClassificationTree? Tree { get; set; }

    /// <summary>获取或设置分类树版本号（官方 <c>version</c>）。</summary>
    [JsonPropertyName("version")]
    public long? Version { get; set; }
}

/// <summary>店铺主页商品分类树（官方 <c>tree</c> 对象，<c>getclassificationtree</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsClassificationTree
{
    /// <summary>获取或设置一级分类列表（官方 <c>level_1</c>）。</summary>
    [JsonPropertyName("level_1")]
    public List<ChannelsClassificationNode>? Level1 { get; set; }

    /// <summary>获取或设置分类树名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置分类树 ID（官方 <c>tree_id</c>）。</summary>
    [JsonPropertyName("tree_id")]
    public long? TreeId { get; set; }
}

/// <summary>
/// 分类树节点（官方 <c>tree.level_1[]</c> / <c>tree.level_1[].level_2[]</c> 元素；两级元素结构一致 ⇒ 共用，
/// 仅 2 级树，一级节点携带 <c>level_2</c> 列表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsClassificationNode
{
    /// <summary>获取或设置分类 ID（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置分类名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置二级分类列表（官方 <c>level_2</c>；仅一级分类节点携带，仅支持 2 级树）。</summary>
    [JsonPropertyName("level_2")]
    public List<ChannelsClassificationNode>? Level2 { get; set; }

    /// <summary>获取或设置分类图标（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}

/// <summary>
/// 获取分类关联的商品ID列表（<c>getclassificationproductlist</c>）请求体。
/// </summary>
/// <remarks>官方契约：请求体为嵌套形态 <c>{"req":{…}}</c>（官方参数表原文，照抄建模）；本接口不支持第三方平台代调用。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetClassificationProductListRequest
{
    /// <summary>获取或设置查询条件（官方 <c>req</c>，必填）。</summary>
    [JsonPropertyName("req")]
    public ChannelsClassificationProductQuery Req { get; set; } = new();
}

/// <summary>分类关联商品查询条件（官方 <c>req</c> 对象，<c>getclassificationproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsClassificationProductQuery
{
    /// <summary>获取或设置一级分类 ID（官方 <c>level_1_id</c>，必填；经「获取在店铺主页展示的商品分类」获取）。</summary>
    [JsonPropertyName("level_1_id")]
    public long Level1Id { get; set; }

    /// <summary>获取或设置二级分类 ID（官方 <c>level_2_id</c>，必填；经同一分类树接口获取）。</summary>
    [JsonPropertyName("level_2_id")]
    public long Level2Id { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_context</c>，必填；由上次请求返回，首页传空串）。</summary>
    [JsonPropertyName("page_context")]
    public string PageContext { get; set; } = string.Empty;

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }
}

/// <summary>获取分类关联的商品ID列表（<c>getclassificationproductlist</c>）响应。</summary>
/// <remarks>官方契约：一个分类最多关联 500 个商品。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetClassificationProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置分类关联的商品 ID 列表（官方 <c>product_ids</c>）。</summary>
    [JsonPropertyName("product_ids")]
    public List<long>? ProductIds { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_context</c>，用于请求下一页；为空表示已到最后一页）。</summary>
    [JsonPropertyName("page_context")]
    public string? PageContext { get; set; }
}

/// <summary>主页申请状态常量（官方 <c>state</c>/<c>audit_state</c> 枚举值，背景图与精选展示位申请共用）。</summary>
public static class ChannelsHomepageApplyStates
{
    /// <summary>审核中。</summary>
    public const int Waiting = 1;

    /// <summary>审核驳回。</summary>
    public const int Rejected = 2;
}

/// <summary>精选展示位展示样式常量（官方 <c>scale</c> 枚举值）。</summary>
public static class ChannelsBannerScales
{
    /// <summary>小图模式（展示位 3–5 个，标题 ≤12 字，描述不可设置）。</summary>
    public const int Small = 1;

    /// <summary>大图模式（标题 ≤40 字，描述 ≤100 字）。</summary>
    public const int Large = 2;
}

/// <summary>精选展示位类型常量（官方 <c>type</c> 枚举值）。</summary>
public static class ChannelsBannerTypes
{
    /// <summary>商品。</summary>
    public const int Product = 1;

    /// <summary>视频号。</summary>
    public const int Finder = 3;

    /// <summary>公众号。</summary>
    public const int OfficialAccount = 4;
}
