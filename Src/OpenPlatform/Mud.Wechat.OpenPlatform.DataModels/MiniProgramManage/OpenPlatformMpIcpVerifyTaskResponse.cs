// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「查询人脸核身任务」应答（<c>query_icp_verifytask</c>；注意官方动词为 POST）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpVerifyTask.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpVerifyTaskResponse : OpenPlatformResponse
{
    /// <summary>核身是否已完成（官方 <c>is_finish</c>，boolean）。</summary>
    [JsonPropertyName("is_finish")]
    public bool? IsFinish { get; set; }

    /// <summary>人脸核身状态（官方 <c>face_status</c>，number）。</summary>
    [JsonPropertyName("face_status")]
    public long? FaceStatus { get; set; }

    /// <summary>随授权核身结果（官方 <c>along_with_auth_result</c>，number）。</summary>
    [JsonPropertyName("along_with_auth_result")]
    public long? AlongWithAuthResult { get; set; }
}
