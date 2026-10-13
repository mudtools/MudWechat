// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Category;

/// <summary>
/// 「获取所有可设置的类目」响应（官方 <c>getallcategories</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaGetAllCategoriesResponse : OpenPlatformResponse
{
    /// <summary>获取或设置类目列表（一级类目数组）。</summary>
    [JsonPropertyName("categories_list")]
    public List<WxaCategoryLevelOne>? CategoriesList { get; set; }
}

/// <summary>
/// 一级类目对象。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaCategoryLevelOne
{
    /// <summary>获取或设置一级类目 ID。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置一级类目名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置二级类目列表。</summary>
    [JsonPropertyName("children")]
    public List<WxaCategoryLevelTwo>? Children { get; set; }
}

/// <summary>
/// 二级类目对象。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaCategoryLevelTwo
{
    /// <summary>获取或设置二级类目 ID。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置二级类目名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置资质信息列表。</summary>
    [JsonPropertyName("qualify_ext_list")]
    public List<WxaQualifyExt>? QualifyExtList { get; set; }
}

/// <summary>
/// 资质信息对象。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaQualifyExt
{
    /// <summary>获取或设置资质 ID。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置资质名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置是否必填（0 否，1 是）。</summary>
    [JsonPropertyName("is_required")]
    public long? IsRequired { get; set; }

    /// <summary>获取或设置示例材料 media_id。</summary>
    [JsonPropertyName("example_media_id")]
    public string? ExampleMediaId { get; set; }
}

/// <summary>
/// 「获取已设置的类目」响应（官方 <c>getcategory</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaGetCategoryResponse : OpenPlatformResponse
{
    /// <summary>获取或设置已设置的类目列表。</summary>
    [JsonPropertyName("categories")]
    public List<WxaSetCategory>? Categories { get; set; }
}

/// <summary>
/// 已设置类目对象。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaSetCategory
{
    /// <summary>获取或设置一级类目 ID。</summary>
    [JsonPropertyName("first_class_id")]
    public long? FirstClassId { get; set; }

    /// <summary>获取或设置一级类目名称。</summary>
    [JsonPropertyName("first_class_name")]
    public string? FirstClassName { get; set; }

    /// <summary>获取或设置二级类目 ID。</summary>
    [JsonPropertyName("second_class_id")]
    public long? SecondClassId { get; set; }

    /// <summary>获取或设置二级类目名称。</summary>
    [JsonPropertyName("second_class_name")]
    public string? SecondClassName { get; set; }

    /// <summary>获取或设置审核状态（0 成功，1 审核中，2 审核不通过，3 名称不符合规范）。</summary>
    [JsonPropertyName("audit_status")]
    public long? AuditStatus { get; set; }

    /// <summary>获取或设置审核不通过原因。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 「根据类型获取类目」请求（官方 <c>getcategoriesbytype</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaGetCategoriesByTypeRequest
{
    /// <summary>获取或设置类目类型（1 主营，2 其他）。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}

/// <summary>
/// 「根据类型获取类目」响应（官方 <c>getcategoriesbytype</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaGetCategoriesByTypeResponse : OpenPlatformResponse
{
    /// <summary>获取或设置类目列表。</summary>
    [JsonPropertyName("categories_list")]
    public List<WxaCategoryLevelOne>? CategoriesList { get; set; }
}

/// <summary>
/// 「添加类目」请求（官方 <c>addcategory</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaAddCategoryRequest
{
    /// <summary>获取或设置一级类目 ID。</summary>
    [JsonPropertyName("first_class_id")]
    public long? FirstClassId { get; set; }

    /// <summary>获取或设置二级类目 ID。</summary>
    [JsonPropertyName("second_class_id")]
    public long? SecondClassId { get; set; }

    /// <summary>获取或设置资质信息列表。</summary>
    [JsonPropertyName("qualify_ext_list")]
    public List<WxaQualifyExtItem>? QualifyExtList { get; set; }
}

/// <summary>
/// 资质信息项（添加/修改类目用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaQualifyExtItem
{
    /// <summary>获取或设置资质 ID。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置资质材料 media_id。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 「删除类目」请求（官方 <c>deletecategory</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaDeleteCategoryRequest
{
    /// <summary>获取或设置一级类目 ID。</summary>
    [JsonPropertyName("first_class_id")]
    public long? FirstClassId { get; set; }

    /// <summary>获取或设置二级类目 ID。</summary>
    [JsonPropertyName("second_class_id")]
    public long? SecondClassId { get; set; }
}

/// <summary>
/// 「修改类目」请求（官方 <c>modifycategory</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaModifyCategoryRequest
{
    /// <summary>获取或设置一级类目 ID。</summary>
    [JsonPropertyName("first_class_id")]
    public long? FirstClassId { get; set; }

    /// <summary>获取或设置二级类目 ID。</summary>
    [JsonPropertyName("second_class_id")]
    public long? SecondClassId { get; set; }

    /// <summary>获取或设置资质信息列表。</summary>
    [JsonPropertyName("qualify_ext_list")]
    public List<WxaQualifyExtItem>? QualifyExtList { get; set; }
}

/// <summary>
/// 「下载扫码购文案模板」响应（官方 <c>qrcodejumpdownload</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaQrCodeJumpDownloadResponse : OpenPlatformResponse
{
    /// <summary>获取或设置文案内容。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 「查询小程序最低基础库版本配置」响应（官方 <c>getweappsupportversion</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaGetWeappSupportVersionResponse : OpenPlatformResponse
{
    /// <summary>获取或设置最低基础库版本。</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

/// <summary>
/// 「设置小程序最低基础库版本」请求（官方 <c>setweappsupportversion</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaSetWeappSupportVersionRequest
{
    /// <summary>获取或设置要设置的最低基础库版本。</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

/// <summary>
/// 「获取公众号关联的小程序」响应（官方 <c>wxamplinkget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaWxampLinkGetResponse : OpenPlatformResponse
{
    /// <summary>获取或设置关联小程序列表。</summary>
    [JsonPropertyName("wx_appid_list")]
    public List<string>? WxAppIdList { get; set; }
}

/// <summary>
/// 「关联小程序」请求（官方 <c>wxamplink</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaWxampLinkRequest
{
    /// <summary>获取或设置要关联的小程序 appid。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>
/// 「解除关联小程序」请求（官方 <c>wxampunlink</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class WxaWxampUnlinkRequest
{
    /// <summary>获取或设置要解除关联的小程序 appid。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
