// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 修改客户备注信息请求体（<c>/cgi-bin/externalcontact/remark</c>）。
/// </summary>
/// <remarks>
/// <para><see cref="Remark"/>、<see cref="Description"/>、<see cref="RemarkCompany"/>、
/// <see cref="RemarkMobiles"/> 与 <see cref="RemarkPicMediaid"/> 不可同时为空。</para>
/// <para><see cref="RemarkCompany"/> 仅在外部联系人为微信用户时有效；填写 <see cref="RemarkMobiles"/>
/// 将整体覆盖旧备注手机号，传一个空字符串（""）即清除全部备注手机号；
/// <see cref="RemarkPicMediaid"/> 经素材管理接口获得。</para>
/// </remarks>
public class UpdateCustomerRemarkRequest
{
    /// <summary>
    /// 获取或设置企业成员的 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人 userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置此用户对外部联系人的备注（最多 20 个字符）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置此用户对外部联系人的描述（最多 150 个字符）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置此用户对外部联系人备注的所属公司名称（最多 20 个字符；仅微信用户有效）。
    /// </summary>
    [JsonPropertyName("remark_company")]
    public string? RemarkCompany { get; set; }

    /// <summary>
    /// 获取或设置此用户对外部联系人备注的手机号列表（整体覆盖；清除全部传空字符串元素）。
    /// </summary>
    [JsonPropertyName("remark_mobiles")]
    public List<string>? RemarkMobiles { get; set; }

    /// <summary>
    /// 获取或设置备注图片的 mediaid（经素材管理接口获得）。
    /// </summary>
    [JsonPropertyName("remark_pic_mediaid")]
    public string? RemarkPicMediaid { get; set; }
}
