// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 超级管理员信息（<c>/shop/wxpay/apply</c> 写侧）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayContactInfo
{
    /// <summary>超级管理员类型（<b>必填</b>）：<c>65</c> 经营者 / 法人、<c>66</c> 经办人。</summary>
    [JsonPropertyName("contact_type")]
    public string? ContactType { get; set; }

    /// <summary>超级管理员邮箱（<b>必填</b>）：须含 <c>@</c> 且符合邮箱格式，用于接收开户邮件与日常业务通知。</summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }

    /// <summary>超级管理员手机（<b>必填</b>）：11 位数字（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    /// <summary>超级管理员证件类型（<b>必填</b>）：固定 <c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c>。</summary>
    [JsonPropertyName("contact_id_doc_type")]
    public string? ContactIdDocType { get; set; }

    /// <summary>超级管理员证件正面照片：管理员类型为经办人时上传。</summary>
    [JsonPropertyName("wxa_contact_id_doc_copy")]
    public WxaShopWxPayPicFile? WxaContactIdDocCopy { get; set; }

    /// <summary>超级管理员证件反面照片：管理员类型为经办人时上传。</summary>
    [JsonPropertyName("wxa_contact_id_doc_copy_back")]
    public WxaShopWxPayPicFile? WxaContactIdDocCopyBack { get; set; }

    /// <summary>超级管理员证件有效期开始时间：管理员类型为经办人时填写。</summary>
    [JsonPropertyName("contact_id_doc_period_begin")]
    public string? ContactIdDocPeriodBegin { get; set; }

    /// <summary>超级管理员证件有效期结束时间：管理员类型为经办人时填写。</summary>
    [JsonPropertyName("contact_id_doc_period_end")]
    public string? ContactIdDocPeriodEnd { get; set; }

    /// <summary>业务办理授权函：管理员类型为经办人时上传；须按示例图打印、信息机打、不可手写并加盖公章。</summary>
    [JsonPropertyName("wxa_business_authorization_letter")]
    public WxaShopWxPayPicFile? WxaBusinessAuthorizationLetter { get; set; }
}
