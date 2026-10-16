// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.RateActivity;

/// <summary>报名费率优惠活动请求·【活动报名信息】报名活动信息·【活动报名材料】1、参见《优惠费率活动对照表》，填写活动报名资料。 2、图片上传。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698774"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationCreateRequestActivityDetailActivityApplyInformationItem
{
    /// <summary>【材料ID】根据活动id（activity_id）所选优惠费率活动，参见《优惠费率活动对照表》填写对应的材料ID。可选取值：`SCHOOL_QUALIFICATION_PROOF` 学校资质证明 / `COOPERATION_QUALIFICATION_PROOF` 合作资质证明 / `ACTIVITY_RATE_COMMITMENT` 优惠费率承诺函 / `CANTEEN_ENVIRONMENT_PHOTO` 食堂内部用餐环境照片 / `CANTEEN_FRONT_PHOTO` 食堂门头照片 / `SCHOOL_LICENSE_PROOF` 办学许可证 / `QUALIFICATION_PROOF` 机构资质证明 / `STORE_TENANCY_PROOF` 门店租赁相关证明 / `STORE_DOOR_PHOTO` 门头照片 / `FOOD_QUALIFICATION_PROOF` 食品经营相关资质 / `PARKING_QUALIFICATION_PROOF` 停车场资质证明 / `PARKINGAREA_PHOTO` 停车场实际经营场景照 / `MEDICAL_INSTITUTION_PRATICE_LICENSE` 医疗机构执业许可证 / `HOSPITAL_FRONTDOOR_PHOTO` 医院门头照 / `TRADE_QUALIFICATION_PROOF` 外贸资质证明（官方字段路径 <c>activity_detail.activity_apply_information.apply_material_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("apply_material_id")]
    public string? ApplyMaterialId { get; set; }

    /// <summary>【材料内容】1、根据所选优惠费率活动，在相应材料名称后提供相关材料，详细参见《报名优惠费率活动对照表》。 2、如需上传图片，请填写通过《图片上传API》预先上传图片生成好的MediaID。最多可上传5张图片。 特殊规则：单个MediaID长度限制为255个字节（官方字段路径 <c>activity_detail.activity_apply_information.apply_material_information</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("apply_material_information")]
    public List<string>? ApplyMaterialInformation { get; set; }

}

/// <summary>报名费率优惠活动请求·【活动报名信息】报名活动信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698774"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationCreateRequestActivityDetail
{
    /// <summary>【活动ID】选择指定活动ID，详细参见《优惠费率活动对照表》（官方字段路径 <c>activity_detail.activity_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>【活动费率】1、参见《优惠费率活动对照表》。 2、若为固定费率，则无需填写该字段 3、若为区间费率，则由从业机构自定义填写，支持两个小数点，需在优惠费率活动ID指定费率范围内，如0.6%（接口无需传%，只需传数字）（官方字段路径 <c>activity_detail.activity_rate</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("activity_rate")]
    public string? ActivityRate { get; set; }

    /// <summary>【活动报名材料】1、参见《优惠费率活动对照表》，填写活动报名资料。 2、图片上传请填写通过《图片上传API》预先上传图片生成的MediaID（官方字段路径 <c>activity_detail.activity_apply_information</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("activity_apply_information")]
    public List<RateActivityApplicationCreateRequestActivityDetailActivityApplyInformationItem>? ActivityApplyInformation { get; set; }

}

/// <summary>报名费率优惠活动请求·【补充信息】补充信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698774"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationCreateRequestAdditionalInformation
{
    /// <summary>【补充说明】根据要求提供额外文本信息（官方字段路径 <c>additional_information.additional_message</c>，选填，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("additional_message")]
    public string? AdditionalMessage { get; set; }

    /// <summary>【补充材料】1、根据要求提供额外补充资料 2、如需上传图片，请填写通过《图片上传API》预先上传图片生成的MediaID，最多可上传5张图片。特殊规则：单个MediaID长度限制为[0,255]个字节（官方字段路径 <c>additional_information.additional_material</c>，选填，类型 <c>array[string(255)]</c>）。</summary>
    [JsonPropertyName("additional_material")]
    public List<string>? AdditionalMaterial { get; set; }

}

/// <summary>报名费率优惠活动请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698774"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationCreateRequest
{
    /// <summary>【从业机构号】微信支付分配的从业机构唯一标识（官方字段路径 <c>acquiring_bank_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>【渠道商户号】微信支付分配的渠道商唯一标识（官方字段路径 <c>channel_id</c>，<b>必填</b>，类型 <c>string(9)</c>）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>【从业机构特约商户号】 1.请填写已完成开户意愿并且已确认的微信支付子商户号。 2.服务商可通过商户开户确认状态API确认子商户是否已完成开户意愿确认（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【活动报名信息】报名活动信息（官方字段路径 <c>activity_detail</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activity_detail")]
    public RateActivityApplicationCreateRequestActivityDetail? ActivityDetail { get; set; }

    /// <summary>【补充信息】补充信息（官方字段路径 <c>additional_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_information")]
    public RateActivityApplicationCreateRequestAdditionalInformation? AdditionalInformation { get; set; }

}

/// <summary>报名费率优惠活动响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698774"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationResponse : WechatPayResponse
{
    /// <summary>【申请单编号】微信支付分配的申请单号（官方字段路径 <c>application_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("application_id")]
    public string? ApplicationId { get; set; }

}

/// <summary>修改单据信息请求·【活动报名修改信息】活动报名修改信息·【活动报名材料】参见报名《优惠费率活动对照表》，填写报名活动所需资质信息。图片上。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698885"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationUpdateRequestActivityDetailModificationActivityApplyInformationItem
{
    /// <summary>【材料ID】根据活动id（activity_id）所选优惠费率活动，参见《优惠费率活动对照表》填写对应的材料ID。可选取值：`SCHOOL_QUALIFICATION_PROOF` 学校资质证明 / `COOPERATION_QUALIFICATION_PROOF` 合作资质证明 / `ACTIVITY_RATE_COMMITMENT` 优惠费率承诺函 / `CANTEEN_ENVIRONMENT_PHOTO` 食堂内部用餐环境照片 / `CANTEEN_FRONT_PHOTO` 食堂门头照片 / `SCHOOL_LICENSE_PROOF` 办学许可证 / `QUALIFICATION_PROOF` 机构资质证明 / `STORE_TENANCY_PROOF` 门店租赁相关证明 / `STORE_DOOR_PHOTO` 门头照片 / `FOOD_QUALIFICATION_PROOF` 食品经营相关资质 / `PARKING_QUALIFICATION_PROOF` 停车场资质证明 / `PARKINGAREA_PHOTO` 停车场实际经营场景照 / `MEDICAL_INSTITUTION_PRATICE_LICENSE` 医疗机构执业许可证 / `HOSPITAL_FRONTDOOR_PHOTO` 医院门头照 / `TRADE_QUALIFICATION_PROOF` 外贸资质证明（官方字段路径 <c>activity_detail_modification.activity_apply_information.apply_material_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("apply_material_id")]
    public string? ApplyMaterialId { get; set; }

    /// <summary>【材料内容】1、根据所选优惠费率活动，在相应材料名称后提供相关材料，详细参见《优惠费率活动对照表》。 2、如需上传图片，请填写通过《图片上传API》预先上传图片生成好的MediaID。最多可上传5张图片。 特殊规则：单个MediaID长度限制为255个字节（官方字段路径 <c>activity_detail_modification.activity_apply_information.apply_material_information</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("apply_material_information")]
    public List<string>? ApplyMaterialInformation { get; set; }

}

/// <summary>修改单据信息请求·【活动报名修改信息】活动报名修改信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698885"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationUpdateRequestActivityDetailModification
{
    /// <summary>【活动费率】参见报名《优惠费率活动对照表》，如为固定费率，则该项不需要填写；若为区间费率，需要由从业机构自定义填写，支持两个小数点，需在优惠费率活动ID指定费率范围内，如0.6%（接口无需传%，只需传数字）（官方字段路径 <c>activity_detail_modification.activity_rate</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("activity_rate")]
    public string? ActivityRate { get; set; }

    /// <summary>【活动报名材料】参见报名《优惠费率活动对照表》，填写报名活动所需资质信息。图片上传请填写通过《图片上传API》预先上传图片生成好的MediaID（官方字段路径 <c>activity_detail_modification.activity_apply_information</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("activity_apply_information")]
    public List<RateActivityApplicationUpdateRequestActivityDetailModificationActivityApplyInformationItem>? ActivityApplyInformation { get; set; }

}

/// <summary>修改单据信息请求·【补充信息】补充信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698885"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationUpdateRequestAdditionalInformation
{
    /// <summary>【补充说明】根据要求提供额外文本信息（官方字段路径 <c>additional_information.additional_message</c>，选填，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("additional_message")]
    public string? AdditionalMessage { get; set; }

    /// <summary>【补充材料】1、根据要求提供额外补充资料 2、如需上传图片，请填写通过《图片上传API》预先上传图片生成的MediaID，最多可上传5张图片。特殊规则：单个MediaID长度限制为[0,255]个字节（官方字段路径 <c>additional_information.additional_material</c>，选填，类型 <c>array[string(255)]</c>）。</summary>
    [JsonPropertyName("additional_material")]
    public List<string>? AdditionalMaterial { get; set; }

}

/// <summary>修改单据信息请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012698885"/></summary>
[HttpJsonSerializable(SerializerClassName = "RateActivity")]
public class RateActivityApplicationUpdateRequest
{
    /// <summary>【活动报名修改信息】活动报名修改信息（官方字段路径 <c>activity_detail_modification</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("activity_detail_modification")]
    public RateActivityApplicationUpdateRequestActivityDetailModification? ActivityDetailModification { get; set; }

    /// <summary>【补充信息】补充信息（官方字段路径 <c>additional_information</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("additional_information")]
    public RateActivityApplicationUpdateRequestAdditionalInformation? AdditionalInformation { get; set; }

}
