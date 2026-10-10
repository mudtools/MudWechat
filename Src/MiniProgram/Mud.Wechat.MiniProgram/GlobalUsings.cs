// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 注：根 Directory.Build.props 已开 ImplicitUsings（System / Collections.Generic / Linq / IO /
// Net.Http / Threading / Threading.Tasks），此处仅补**非隐式**的组件面与本产品线命名空间。
//
// 令牌基座来自 Mud.Wechat.OfficialAccount.Abstractions（经本线 Abstractions 传递引用，见设计方案 §3.2）：
// 小程序与公众号同属微信公众平台、同一 /cgi-bin/token 端点、同一令牌域 ⇒ 零增量复用。
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.MiniProgram.DataModels;
global using Mud.Wechat.MiniProgram.DataModels.Auth;
global using Mud.Wechat.MiniProgram.DataModels.Charge;
global using Mud.Wechat.MiniProgram.DataModels.DataAnalysis;
global using Mud.Wechat.MiniProgram.DataModels.DynamicMessage;
global using Mud.Wechat.MiniProgram.DataModels.FaceVerify;
global using Mud.Wechat.MiniProgram.DataModels.HardwareDevice;
global using Mud.Wechat.MiniProgram.DataModels.Kf;
global using Mud.Wechat.MiniProgram.DataModels.LaborUse;
global using Mud.Wechat.MiniProgram.DataModels.NearbyPoi;
global using Mud.Wechat.MiniProgram.DataModels.Operation;
global using Mud.Wechat.MiniProgram.DataModels.Plugin;
global using Mud.Wechat.MiniProgram.DataModels.QrCodeLink;
global using Mud.Wechat.MiniProgram.DataModels.RedPacketCover;
global using Mud.Wechat.MiniProgram.DataModels.Search;
global using Mud.Wechat.MiniProgram.DataModels.Security;
global using Mud.Wechat.MiniProgram.DataModels.ServiceMarket;
global using Mud.Wechat.MiniProgram.DataModels.Soter;
global using Mud.Wechat.MiniProgram.DataModels.Student;
global using Mud.Wechat.MiniProgram.DataModels.SubscribeMessage;
global using Mud.Wechat.OfficialAccount.Abstractions;
global using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
