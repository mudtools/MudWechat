// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信 API 模块枚举（对齐 FeishuModule）。
/// </summary>
/// <remarks>
/// 令牌签发客户端（gettoken / get_provider_token / get_suite_token / get_corp_token）
/// 位于 Abstractions 的 Authentication 注册组，随令牌底座自动注册，不经本枚举。
/// </remarks>
public enum WechatModule
{
    /// <summary>授权流接口（get_pre_auth_code / set_session_info / get_permanent_code / get_auth_info）。</summary>
    Authentication,

    /// <summary>通讯录（成员管理域：公共读取面 + 自建/第三方/代开发能力差异端点）。</summary>
    Contact,

    /// <summary>客户联系（企业服务人员管理域 + 客户管理域：公共面 + 第三方/代开发能力差异端点）。</summary>
    ExternalContact,

    /// <summary>消息推送等（预留）。</summary>
    Message,
}
