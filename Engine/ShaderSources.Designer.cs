﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cubed {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ShaderSources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ShaderSources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cubed.ShaderSources", typeof(ShaderSources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///uniform vec4 ambient;
        ///uniform sampler2D texture;
        ///uniform sampler2D lightmap;
        ///varying vec2 texCoord;
        ///varying vec2 lightTexCoord;
        ///
        ///void main() {
        ///	gl_FragColor = 
        ///		texture2D(texture, texCoord) * 
        ///		max(texture2D(lightmap, lightTexCoord), ambient);
        ///}.
        /// </summary>
        internal static string MapFragment {
            get {
                return ResourceManager.GetString("MapFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///attribute vec2 inTexCoord;
        ///attribute vec2 inLightCoord;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///uniform mat4 textureMatrix;
        ///
        ///varying vec2 texCoord;
        ///varying vec2 lightTexCoord;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	texCoord = (textureMatrix * vec4(inTexCoord, 0.0, 0.0)).xy;
        ///	lightTexCoord = inLightCoord;
        ///	gl_Position = m * v;
        ///}.
        /// </summary>
        internal static string MapVertex {
            get {
                return ResourceManager.GetString("MapVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///uniform vec4 diffuseColor;
        ///uniform sampler2D texture;
        ///uniform bool discardPass;
        ///uniform float lightMult;
        ///
        ///varying vec3 normal;
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	vec4 c = texture2D(texture, texCoord) * diffuseColor;
        ///	c = vec4(mix(c.rgb * dot(normal, vec3(0.0, 1.0, 0.0)), c.rgb, lightMult), c.a);
        ///	if(discardPass){
        ///		if(c.a &lt; 1.0) discard;
        ///	}
        ///	gl_FragColor = c;
        ///}.
        /// </summary>
        internal static string MeshFragment {
            get {
                return ResourceManager.GetString("MeshFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///attribute vec3 inNormal;
        ///attribute vec2 inTexCoord;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///uniform mat4 textureMatrix;
        ///
        ///varying vec3 normal;
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	vec4 n = vec4(inNormal, 0.0);
        ///	texCoord = (textureMatrix * vec4(inTexCoord, 0.0, 0.0)).xy;
        ///	normal = (m * n).xyz;
        ///	gl_Position = m *  [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string MeshVertex {
            get {
                return ResourceManager.GetString("MeshVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inFirstPosition;
        ///attribute vec3 inSecondPosition;
        ///attribute vec3 inFirstNormal;
        ///attribute vec3 inSecondNormal;
        ///attribute vec2 inTexCoord;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///uniform mat4 textureMatrix;
        ///uniform float delta;
        ///
        ///varying vec3 normal;
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(mix(inFirstPosition, inSecondPosition, delta), 1.0);
        ///	vec [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string MorphMeshVertex {
            get {
                return ResourceManager.GetString("MorphMeshVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///uniform sampler2D texture;
        ///varying vec2 texCoord;
        ///varying vec4 color;
        ///
        ///void main() {
        ///	gl_FragColor = 
        ///		texture2D(texture, texCoord) * color;
        ///}.
        /// </summary>
        internal static string ShadowPassFragment {
            get {
                return ResourceManager.GetString("ShadowPassFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///attribute vec2 inTexCoord;
        ///attribute vec4 inColor;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 textureMatrix;
        ///
        ///
        ///varying vec2 texCoord;
        ///varying vec4 color;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	texCoord = (textureMatrix * vec4(inTexCoord, 0.0, 0.0)).xy;
        ///	color = inColor;
        ///	gl_Position = m * v;
        ///}.
        /// </summary>
        internal static string ShadowPassVertex {
            get {
                return ResourceManager.GetString("ShadowPassVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на .
        /// </summary>
        internal static string SkinnedMeshVertex {
            get {
                return ResourceManager.GetString("SkinnedMeshVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///uniform vec4 diffuseColor;
        ///uniform sampler2D texture;
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	gl_FragColor = texture2D(texture, texCoord) * diffuseColor;
        ///}.
        /// </summary>
        internal static string SkyFragment {
            get {
                return ResourceManager.GetString("SkyFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///attribute vec2 inTexCoord;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///uniform mat4 textureMatrix;
        ///
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	texCoord = (textureMatrix * vec4(inTexCoord, 0.0, 0.0)).xy;
        ///	gl_Position = m * v;
        ///}.
        /// </summary>
        internal static string SkyVertex {
            get {
                return ResourceManager.GetString("SkyVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///uniform vec4 diffuseColor;
        ///uniform sampler2D texture;
        ///uniform bool discardPass;
        ///
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	vec4 c = texture2D(texture, texCoord) * diffuseColor;
        ///	if(discardPass){
        ///		if(c.a &lt; 1.0) discard;
        ///	}
        ///	gl_FragColor = c;
        ///}.
        /// </summary>
        internal static string SpriteFragment {
            get {
                return ResourceManager.GetString("SpriteFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///attribute vec2 inTexCoord;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///uniform mat4 textureMatrix;
        ///
        ///varying vec2 texCoord;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	texCoord = (textureMatrix * vec4(inTexCoord, 0.0, 0.0)).xy;
        ///	gl_Position = m * v;
        ///}.
        /// </summary>
        internal static string SpriteVertex {
            get {
                return ResourceManager.GetString("SpriteVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///uniform vec4 diffuseColor;
        ///
        ///void main() {
        ///	gl_FragColor = diffuseColor;
        ///}.
        /// </summary>
        internal static string WireCubeFragment {
            get {
                return ResourceManager.GetString("WireCubeFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///
        ///void main() {
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	gl_Position = m * v;
        ///}.
        /// </summary>
        internal static string WireCubeVertex {
            get {
                return ResourceManager.GetString("WireCubeVertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///varying vec4 color;
        ///
        ///void main() {
        ///	gl_FragColor = color;
        ///}.
        /// </summary>
        internal static string WireGridFragment {
            get {
                return ResourceManager.GetString("WireGridFragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на #version 120
        ///
        ///attribute vec3 inPosition;
        ///attribute vec4 inColor;
        ///
        ///uniform mat4 projectionMatrix;
        ///uniform mat4 cameraMatrix;
        ///uniform mat4 entityMatrix;
        ///
        ///varying vec4 color;
        ///
        ///void main() {
        ///	color = inColor;
        ///	
        ///	mat4 m = projectionMatrix * cameraMatrix * entityMatrix;
        ///	vec4 v = vec4(inPosition, 1.0);
        ///	gl_Position = m * v;
        ///}.
        /// </summary>
        internal static string WireGridVertex {
            get {
                return ResourceManager.GetString("WireGridVertex", resourceCulture);
            }
        }
    }
}
