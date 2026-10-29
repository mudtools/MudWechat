// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 流量主统计回包内的 <c>base_resp</c> 错误信封（官方三端点回包共用；与顶层 <c>errcode/errmsg</c> 令牌错误并存）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherBaseResp
{
    /// <summary>获取或设置错误信息（官方 <c>err_msg</c>，成功为 <c>ok</c>）。</summary>
    [JsonPropertyName("err_msg")]
    public string? ErrMsg { get; set; }

    /// <summary>获取或设置错误码（官方 <c>ret</c>，成功为 <c>0</c>）。</summary>
    [JsonPropertyName("ret")]
    public int Ret { get; set; }
}
