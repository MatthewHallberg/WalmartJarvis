//implements cv::omnidir::initUndistortRectifyMap() for omnidirectional lenses
//https://github.com/opencv/opencv_contrib/blob/master/modules/ccalib/src/omnidir.cpp

Shader "Custom/Undistort"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InverseMatrix ("InverseMatrix", Vector) = (0, 0, 0, 0)
        _F1 ("F1", Float) = 0
		_F2 ("F2", Float) = 0
		_OX ("OX", Float) = 0
		_OY ("OY", Float) = 0
        _S ("S", Float) = 0
		_K1 ("K1", Float) = 0
		_K2 ("K2", Float) = 0
		_P1 ("P1", Float) = 0
		_P2 ("P2", Float) = 0
		_Omni ("Omni", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Interpolation.cginc"

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			struct v2f {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            uniform float3 _InverseMatrix;
            //cam matrix values
			uniform float _F1;
			uniform float _F2;
			uniform float _OX;
			uniform float _OY;
            uniform float _S;
			//DistCoeffsValues from camera calibration
			uniform float _K1;
			uniform float _K2;
			uniform float _P1;
			uniform float _P2;
			uniform float _Omni;

            fixed4 frag (v2f i) : COLOR {

				float2 f = {_F1,_F2}; //physical camera focal length
                float2 c = {_OX,_OY}; //optical center
                float2 s = _S;
                float2 k = {_K1, _K2}; //radial distortion
                float2 p = {_P1, _P2}; //tangential distortion
                float _xi = _Omni;
                float2 imageSize =  float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);

                float3x3 iKR = {_InverseMatrix[0], 0, _InverseMatrix[1],
                                0, _InverseMatrix[0] , _InverseMatrix[2],
                                0, 0, 1};

                //remap UVs to openCV image coords
                float2 pos;
                pos.x = i.uv.x * imageSize.x;
                pos.y = remap(1,0,0,imageSize.y,i.uv.y);

                //multiply by inverse of camera matrix
                float _x = pos.y*iKR[0][1] + iKR[0][2] + pos.x* iKR[0][0];
                float _y = pos.y*iKR[1][1] + iKR[1][2] + pos.x* iKR[1][0];
                float _w = pos.y*iKR[2][1] + iKR[2][2] + pos.x* iKR[2][0];

                float r = sqrt(_x*_x + _y*_y + _w*_w);
                float Xs = _x / r;
                float Ys = _y / r;
                float Zs = _w / r;

                float xu = Xs / (Zs + _xi);
                float yu = Ys / (Zs + _xi);

                float r2 = xu*xu + yu*yu;
                float r4 = r2*r2;
                float xd = (1+k[0]*r2+k[1]*r4)*xu + 2*p[0]*xu*yu + p[1]*(r2+2*xu*xu);
                float yd = (1+k[0]*r2+k[1]*r4)*yu + p[0]*(r2+2*yu*yu) + 2*p[1]*xu*yu;

                float u = f[0]*xd + c[0] + s*yd;
                float v = f[1]*yd + c[1];

                float2 distortedUV = {u,v};
                distortedUV.x = u / imageSize.x;
                distortedUV.y = remap(0,imageSize.y,1,0,v);

				fixed4 main = tex2D(_MainTex, distortedUV);

                //make pixels beyond uv black (texture should be set to clamp)
                if (distortedUV.x <= 0 || distortedUV.y <= 0 || distortedUV.x >= 1 || distortedUV.y >= 1){
                    main = fixed4 (0,0,0,1);
                }

				return main;     
            }
            ENDCG
        }
    }
}
