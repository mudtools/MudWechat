// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件查询获客链接使用详情响应体（<c>/cgi-bin/externalcontact/customer_acquisition/statistic</c>）。
/// </summary>
/// <remarks>
/// 组件口径：获客链接需携带 <c>comp_scene</c> 参数方可计入组件数据。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentLinkStatisticResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置点击链接的客户数。
    /// </summary>
    [JsonPropertyName("click_link_customer_cnt")]
    public long? ClickLinkCustomerCnt { get; set; }

    /// <summary>
    /// 获取或设置新增客户数。
    /// </summary>
    [JsonPropertyName("new_customer_cnt")]
    public long? NewCustomerCnt { get; set; }
}
