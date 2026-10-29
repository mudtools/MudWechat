// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 是否申请版权保护取值（<c>copyright.apply_for_copyright_protection</c>）。
/// </summary>
/// <remarks>
/// <para><b>2026-07-28 12:00 起默认传 <c>1</c> 且不支持修改</b>（官方原文）；填 <c>0</c> 时必须上传权利声明/播放授权材料（<c>authorized_material_id</c>）。受版权保护的剧目才能被授权（见「增加版权授权」）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
public static class WxaVodApplyForCopyrightProtection
{
    /// <summary>不申请版权保护（<c>0</c>，须上传权利声明/播放授权材料）。</summary>
    public const int NotApply = 0;

    /// <summary>申请版权保护（<c>1</c>）。</summary>
    public const int Apply = 1;
}
