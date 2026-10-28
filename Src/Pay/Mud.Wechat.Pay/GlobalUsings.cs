// 2026-10 包合并：原 Pay.Abstractions / Pay.DataModels / Pay.V2 / Pay.V2.DataModels 四工程并入
// 本工程（命名空间不变），各工程的全局导入取并集落此。
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.Abstractions.Contracts;
global using Mud.Wechat.Pay.Abstractions.Configuration;
global using Mud.Wechat.Pay.Abstractions.Credential;
global using Mud.Wechat.Pay.Abstractions.Transport;
global using Mud.Wechat.Pay.DataModels.Transactions;
global using Mud.Wechat.Pay.V2.DataModels.PayV2;
global using Mud.Wechat.Pay.V2.Security;
global using Mud.Wechat.Pay.V2.Transport;
global using System.Globalization;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using System.Xml.Linq;
