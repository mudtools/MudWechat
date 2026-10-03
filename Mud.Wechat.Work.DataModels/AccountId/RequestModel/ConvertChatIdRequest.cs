// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// 群 ID 转换请求体（<c>/cgi-bin/idconvert/chatid</c>）：
/// 将升级前企业主体的群 ID 转换成升级后服务商主体的群 ID；传入升级后的群 ID 则原样返回。
/// </summary>
/// <remarks>
/// 官方限制：仅代开发应用可调用；需先调用申请群 ID 的升级接口后方可调用本接口，到达指定的升级时间后无法调用；
/// 应用需具有「客户联系 -&gt; 基础客户信息」权限；群主需在应用的可见范围中；chat_id_list 最多输入 100 个。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ConvertChatIdRequest
{
    /// <summary>
    /// 获取或设置需要转换的群 ID 列表（官方必填，最多输入 100 个）。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }
}
