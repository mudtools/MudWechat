// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 获取跳转小程序商城的直播观众信息请求体（<c>/cgi-bin/living/get_living_share_info</c>；三种应用类型请求形态一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class GetLivingShareInfoRequest
{
    /// <summary>
    /// 获取或设置「推广产品」直播观众跳转小程序商城时在小程序 path 中携带的 ww_share_code 参数（官方必填）。
    /// <para>ww_share_code 五分钟内有效；跳转的小程序需要与企业有绑定关系。</para>
    /// </summary>
    [JsonPropertyName("ww_share_code")]
    public string? WwShareCode { get; set; }
}
