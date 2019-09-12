using System;
using System.Collections.Generic;

namespace Devdog.General.ThirdParty.FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.Devdog_General_LocalizedAudioClipInfo_DirectConverter Register_Devdog_General_LocalizedAudioClipInfo;
    }
}

namespace Devdog.General.ThirdParty.FullSerializer.Speedup {
    public class Devdog_General_LocalizedAudioClipInfo_DirectConverter : fsDirectConverter<Devdog.General.LocalizedAudioClipInfo> {
        protected override fsResult DoSerialize(Devdog.General.LocalizedAudioClipInfo model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "audioClip", model.audioClip);
            result += SerializeMember(serialized, null, "volume", model.volume);
            result += SerializeMember(serialized, null, "pitch", model.pitch);
            result += SerializeMember(serialized, null, "loop", model.loop);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Devdog.General.LocalizedAudioClipInfo model) {
            var result = fsResult.Success;

            var t0 = model.audioClip;
            result += DeserializeMember(data, null, "audioClip", out t0);
            model.audioClip = t0;

            var t1 = model.volume;
            result += DeserializeMember(data, null, "volume", out t1);
            model.volume = t1;

            var t2 = model.pitch;
            result += DeserializeMember(data, null, "pitch", out t2);
            model.pitch = t2;

            var t3 = model.loop;
            result += DeserializeMember(data, null, "loop", out t3);
            model.loop = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new Devdog.General.LocalizedAudioClipInfo();
        }
    }
}
