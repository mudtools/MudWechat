// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 修改打卡规则请求体（<c>/cgi-bin/checkin/update_checkin_option</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方更新语义：打卡规则仅可由该规则的创建应用修改；修改打卡规则时 group 须传入 groupid，否则会报错；
/// 对 group.* 一级的字段——数组字段「不传/传空 = 不更新、传值 = 覆盖」；非数组字段「传值 = 覆盖（含递归所有字段）、不传 = 不更新」；
/// 若想清空 group.* 一级的数组字段须使用清空打卡规则数组元素接口（<see cref="ClearCheckinOptionArrayFieldRequest"/>），
/// 非数组字段直接传入空元素即可。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class UpdateCheckinOptionRequest
{
    /// <summary>
    /// 获取或设置打卡规则详细定义（官方视情况必填；修改时 group.groupid 须传入，否则会报错）。
    /// </summary>
    [JsonPropertyName("group")]
    public CheckinRuleGroup? Group { get; set; }

    /// <summary>
    /// 获取或设置是否立即生效（默认为 false）。
    /// </summary>
    [JsonPropertyName("effective_now")]
    public bool? EffectiveNow { get; set; }
}
