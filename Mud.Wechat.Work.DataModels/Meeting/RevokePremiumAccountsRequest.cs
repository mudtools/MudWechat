// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 取消高级功能账号请求体（<c>/cgi-bin/meeting/vip/submit_batch_del_job</c>；该接口用于撤销分配应用可见范围企业成员的高级功能）。
/// </summary>
/// <remarks>
/// <para>官方限制：userid_list 单次操作最多限制 100 个；自建应用需配置到「协作 - 会议 - 可调用接口的应用」中（代开发/第三方暂不支持）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RevokePremiumAccountsRequest
{
    /// <summary>获取或设置要撤销分配高级功能的企业成员 userid 列表（官方必填；单次操作最多限制 100 个）。</summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }
}
