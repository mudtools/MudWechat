// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 字段详情（官方 Field / AddField / UpdateField；添加字段与更新字段的请求元素、添加字段 / 更新字段 / 查询字段的响应 <c>fields</c> 元素共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方明示「<b>字段属性与字段类型是匹配的，一种字段类型对应一种字段属性</b>」，故每次只应设置与 <c>field_type</c> 匹配的那一个 <c>property_*</c> 字段。</para>
/// <para>
/// 官方「更新字段」文档另列出 <c>property_text</c>（文本）、<c>property_created_user</c>（创建人）、<c>property_modified_user</c>（最后编辑人）三个字段属性，官方明确标注其类型为「object」且说明「为空」，无任何可传输字段，故本模型不予承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetField
{
    /// <summary>
    /// 获取或设置字段 ID（官方 <c>field_id</c>）。
    /// 添加字段请求不传该字段；更新字段时必填且不能被更新。
    /// </summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>获取或设置字段标题（官方 <c>field_title</c>）。</summary>
    [JsonPropertyName("field_title")]
    public string? FieldTitle { get; set; }

    /// <summary>
    /// 获取或设置字段类型（官方 <c>field_type</c>），见官方 <c>FieldType</c>。
    /// 官方取值：<c>FIELD_TYPE_TEXT</c> 文本、<c>FIELD_TYPE_NUMBER</c> 数字、<c>FIELD_TYPE_CHECKBOX</c> 复选框、<c>FIELD_TYPE_DATE_TIME</c> 日期、<c>FIELD_TYPE_IMAGE</c> 图片、<c>FIELD_TYPE_ATTACHMENT</c> 文件、<c>FIELD_TYPE_USER</c> 成员、<c>FIELD_TYPE_URL</c> 超链接、<c>FIELD_TYPE_SELECT</c> 多选、<c>FIELD_TYPE_CREATED_USER</c> 创建人、<c>FIELD_TYPE_MODIFIED_USER</c> 最后编辑人、<c>FIELD_TYPE_CREATED_TIME</c> 创建时间、<c>FIELD_TYPE_MODIFIED_TIME</c> 最后编辑时间、<c>FIELD_TYPE_PROGRESS</c> 进度、<c>FIELD_TYPE_PHONE_NUMBER</c> 电话、<c>FIELD_TYPE_EMAIL</c> 邮件、<c>FIELD_TYPE_SINGLE_SELECT</c> 单选、<c>FIELD_TYPE_REFERENCE</c> 关联、<c>FIELD_TYPE_LOCATION</c> 地理位置、<c>FIELD_TYPE_FORMULA</c> 公式、<c>FIELD_TYPE_CURRENCY</c> 货币、<c>FIELD_TYPE_WWGROUP</c> 群、<c>FIELD_TYPE_AUTONUMBER</c> 自动编号、<c>FIELD_TYPE_PERCENTAGE</c> 百分数、<c>FIELD_TYPE_BARCODE</c> 条码。
    /// 更新字段时 <c>field_type</c> 必须为原属性（该接口不能更新字段类型）。
    /// </summary>
    [JsonPropertyName("field_type")]
    public string? FieldType { get; set; }

    /// <summary>获取或设置数字类型的字段属性（官方 <c>property_number</c>）。</summary>
    [JsonPropertyName("property_number")]
    public SmartSheetNumberFieldProperty? PropertyNumber { get; set; }

    /// <summary>获取或设置复选框类型的字段属性（官方 <c>property_checkbox</c>）。</summary>
    [JsonPropertyName("property_checkbox")]
    public SmartSheetCheckboxFieldProperty? PropertyCheckbox { get; set; }

    /// <summary>获取或设置日期类型的字段属性（官方 <c>property_date_time</c>）。</summary>
    [JsonPropertyName("property_date_time")]
    public SmartSheetDateTimeFieldProperty? PropertyDateTime { get; set; }

    /// <summary>获取或设置文件类型的字段属性（官方 <c>property_attachment</c>）。</summary>
    [JsonPropertyName("property_attachment")]
    public SmartSheetAttachmentFieldProperty? PropertyAttachment { get; set; }

    /// <summary>获取或设置人员类型的字段属性（官方 <c>property_user</c>）。</summary>
    [JsonPropertyName("property_user")]
    public SmartSheetUserFieldProperty? PropertyUser { get; set; }

    /// <summary>获取或设置超链接类型的字段属性（官方 <c>property_url</c>）。</summary>
    [JsonPropertyName("property_url")]
    public SmartSheetUrlFieldProperty? PropertyUrl { get; set; }

    /// <summary>获取或设置多选类型的字段属性（官方 <c>property_select</c>）。</summary>
    [JsonPropertyName("property_select")]
    public SmartSheetSelectFieldProperty? PropertySelect { get; set; }

    /// <summary>获取或设置创建时间类型的字段属性（官方 <c>property_created_time</c>）。</summary>
    [JsonPropertyName("property_created_time")]
    public SmartSheetCreatedTimeFieldProperty? PropertyCreatedTime { get; set; }

    /// <summary>获取或设置最后编辑时间类型的字段属性（官方 <c>property_modified_time</c>）。</summary>
    [JsonPropertyName("property_modified_time")]
    public SmartSheetModifiedTimeFieldProperty? PropertyModifiedTime { get; set; }

    /// <summary>获取或设置进度类型的字段属性（官方 <c>property_progress</c>）。</summary>
    [JsonPropertyName("property_progress")]
    public SmartSheetProgressFieldProperty? PropertyProgress { get; set; }

    /// <summary>获取或设置单选类型的字段属性（官方 <c>property_single_select</c>）。</summary>
    [JsonPropertyName("property_single_select")]
    public SmartSheetSingleSelectFieldProperty? PropertySingleSelect { get; set; }

    /// <summary>获取或设置关联类型的字段属性（官方 <c>property_reference</c>）。</summary>
    [JsonPropertyName("property_reference")]
    public SmartSheetReferenceFieldProperty? PropertyReference { get; set; }

    /// <summary>获取或设置地理位置类型的字段属性（官方 <c>property_location</c>）。</summary>
    [JsonPropertyName("property_location")]
    public SmartSheetLocationFieldProperty? PropertyLocation { get; set; }

    /// <summary>获取或设置自动编号类型的字段属性（官方 <c>property_auto_number</c>）。</summary>
    [JsonPropertyName("property_auto_number")]
    public SmartSheetAutoNumberFieldProperty? PropertyAutoNumber { get; set; }

    /// <summary>获取或设置货币类型的字段属性（官方 <c>property_currency</c>）。</summary>
    [JsonPropertyName("property_currency")]
    public SmartSheetCurrencyFieldProperty? PropertyCurrency { get; set; }

    /// <summary>获取或设置群类型的字段属性（官方 <c>property_ww_group</c>）。</summary>
    [JsonPropertyName("property_ww_group")]
    public SmartSheetWwGroupFieldProperty? PropertyWwGroup { get; set; }

    /// <summary>获取或设置百分数类型的字段属性（官方 <c>property_percentage</c>）。</summary>
    [JsonPropertyName("property_percentage")]
    public SmartSheetPercentageFieldProperty? PropertyPercentage { get; set; }

    /// <summary>获取或设置条码类型的字段属性（官方 <c>property_barcode</c>）。</summary>
    [JsonPropertyName("property_barcode")]
    public SmartSheetBarcodeFieldProperty? PropertyBarcode { get; set; }
}
