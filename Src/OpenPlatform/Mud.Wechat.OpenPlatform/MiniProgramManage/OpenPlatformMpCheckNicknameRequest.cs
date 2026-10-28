// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 微信认证名称检测请求（<c>POST /cgi-bin/wxverify/checkwxverifynickname</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/checkNickName.html"/></para>
/// <para>官方约束：该接口只允许通过 API 创建的小程序使用；令牌面为 <c>authorizer_access_token</c>，权限集 id 3 或 31。</para>
/// <para>官方错误码：<c>-1</c>（系统繁忙）/ <c>40001</c>（access_token 无效）/ <c>41033</c>（非 API 创建小程序）/ <c>53010~53022</c>（名称规则违规）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpCheckNicknameRequest
{
    /// <summary>名称（昵称），必填。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }
}
