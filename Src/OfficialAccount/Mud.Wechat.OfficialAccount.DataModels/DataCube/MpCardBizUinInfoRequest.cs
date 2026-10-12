// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「卡券帐号级统计」请求（官方 <c>getcardbizuininfo</c>；在通用日期区间上追加卡券来源过滤）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardBizUinInfoRequest : MpDateRangeRequest
{
    /// <summary>获取或设置卡券来源（官方 <c>cond_source</c>；0=全部，1=普通券（公众号）来源，2=小程序来源）。</summary>
    [JsonPropertyName("cond_source")]
    public int CardSource { get; set; }
}
