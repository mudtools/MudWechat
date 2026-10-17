// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取可展示小程序列表」应答（<c>getwxamplinkforshow</c>，官方为 GET，每页最多 20 条）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/subscribe-component/getLinkForShow.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpLinkForShowResponse : OpenPlatformResponse
{
    /// <summary>可展示小程序总数（官方 <c>total_num</c>，number）。</summary>
    [JsonPropertyName("total_num")]
    public long TotalNum { get; set; }

    /// <summary>可展示小程序列表（官方 <c>biz_info_list</c>，array）。</summary>
    [JsonPropertyName("biz_info_list")]
    public List<OpenPlatformMpShowBizInfo>? BizInfoList { get; set; }
}
