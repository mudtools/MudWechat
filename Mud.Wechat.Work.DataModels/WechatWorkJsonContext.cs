// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Text.Json.Serialization;

namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// 企业微信数据模型 AOT JsonContext（对齐 FeishuApiResultJsonContext / {Module}JsonContext 模式）。
/// 经主包 <c>WechatJsonResolverExtensions</c> 合并进组件序列化管线。
/// </summary>
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(WechatWorkResponse))]
[JsonSerializable(typeof(WechatChatbotResponse<WechatWorkResponse>))]
[JsonSerializable(typeof(InternalAppAuthentication.GetTokenResponse))]
[JsonSerializable(typeof(ProviderAuthentication.GetProviderTokenRequest))]
[JsonSerializable(typeof(ProviderAuthentication.GetProviderTokenResponse))]
[JsonSerializable(typeof(ProviderAuthentication.GetSuiteTokenRequest))]
[JsonSerializable(typeof(ProviderAuthentication.GetSuiteTokenResponse))]
[JsonSerializable(typeof(ProviderAuthentication.GetPreAuthCodeResponse))]
[JsonSerializable(typeof(ProviderAuthentication.GetPermanentCodeRequest))]
[JsonSerializable(typeof(ProviderAuthentication.GetPermanentCodeResponse))]
[JsonSerializable(typeof(ProviderAuthentication.GetAuthInfoRequest))]
[JsonSerializable(typeof(ProviderAuthentication.GetAuthInfoResponse))]
[JsonSerializable(typeof(ProviderAuthentication.SetSessionInfoRequest))]
[JsonSerializable(typeof(ProviderAuthentication.GetCustomizedAuthUrlRequest))]
[JsonSerializable(typeof(ProviderAuthentication.GetCustomizedAuthUrlResponse))]
[JsonSerializable(typeof(ProviderAuthentication.SessionInfo))]
[JsonSerializable(typeof(ProviderAuthentication.Agent))]
[JsonSerializable(typeof(ProviderAuthentication.EditionAgent))]
[JsonSerializable(typeof(ProviderAuthentication.Edition))]
[JsonSerializable(typeof(ProviderAuthentication.Privilege))]
[JsonSerializable(typeof(ProviderAuthentication.SharedFrom))]
[JsonSerializable(typeof(ProviderAuthentication.AuthCorpDetailInfo))]
[JsonSerializable(typeof(ProviderAuthentication.AuthCorpDetailInfoExt))]
[JsonSerializable(typeof(ProviderAuthentication.CorpExName))]
[JsonSerializable(typeof(ProviderAuthentication.DealerCorpInfo))]
[JsonSerializable(typeof(ProviderAuthentication.AuthInfo))]
[JsonSerializable(typeof(ProviderAuthentication.AuthUserInfo))]
[JsonSerializable(typeof(ProviderAuthentication.RegisterCodeInfo))]
[JsonSerializable(typeof(CorpTokenAuthentication.GetCorpTokenRequest))]
[JsonSerializable(typeof(CorpTokenAuthentication.GetCorpTokenResponse))]
public sealed partial class WechatWorkJsonContext : JsonSerializerContext
{
}
#endif
