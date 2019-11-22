using System;

namespace wavegenerator
{
    public class SectionId
    {
        public SectionId(int section, int channel)
        {
            Section = section;
            Channel = channel;
        }
        public int Section { get; }
        public int Channel { get; }

        public override bool Equals(object obj)
        {
            if(Settings.Instance.ChannelSettings.Length == 1)
            {
                return obj is SectionId id &&
                       Section == id.Section;
            }
            else
            {
                return obj is SectionId id &&
                       Section == id.Section &&
                       Channel == id.Channel;
            }
        }

        public override int GetHashCode()
        {
            if(Settings.Instance.ChannelSettings.Length == 1)
            {
                return HashCode.Combine(Section);
            }
            else
            {
                return HashCode.Combine(Section, Channel);
            }
        }
    }
}
