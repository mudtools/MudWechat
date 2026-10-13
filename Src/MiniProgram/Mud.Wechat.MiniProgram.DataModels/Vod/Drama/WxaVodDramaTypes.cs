// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目类型取值（<c>drama_type</c>）。
/// </summary>
/// <remarks>
/// <para><b>只有 <c>Cartoon=false</c> 的真人剧要演员信息</b>（官方原文）：<c>2</c> 真人时 <c>actor_list</c> 必填，<c>1</c> 漫剧与 <c>3</c> 数字真人均无需填写「actor_list-演员信息」。</para>
/// <para><b><c>3</c> 数字真人 2026-04-20 11:00 起生效</b>（官方接口变更日志：2026-04-13）⇒ 照录现值集，不补「可能的 4」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
public static class WxaVodDramaTypes
{
    /// <summary>漫剧：基于漫画、动画或绘图风格制作，不涉及真人出演（<c>1</c>）。</summary>
    public const int Comic = 1;

    /// <summary>真人：真实演员出演、实景拍摄或特效辅助制作，须上传演职人员信息（<c>2</c>）。</summary>
    public const int RealPerson = 2;

    /// <summary>数字真人：以计算机图形技术与 AI 算法生成的虚拟角色出演（<c>3</c>，2026-04-20 起生效），无需演职人员信息。</summary>
    public const int DigitalHuman = 3;
}
