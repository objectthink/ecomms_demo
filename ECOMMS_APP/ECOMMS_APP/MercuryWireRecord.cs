﻿// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: mercurywirerecord.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MercuryShim
{

    /// <summary>Holder for reflection information generated from mercurywirerecord.proto</summary>
    public static partial class MercurywirerecordReflection
    {

        #region Descriptor
        /// <summary>File descriptor for mercurywirerecord.proto</summary>
        public static pbr::FileDescriptor Descriptor
        {
            get { return descriptor; }
        }
        private static pbr::FileDescriptor descriptor;

        static MercurywirerecordReflection()
        {
            byte[] descriptorData = global::System.Convert.FromBase64String(
                string.Concat(
                  "ChdtZXJjdXJ5d2lyZXJlY29yZC5wcm90bxIMbWVyY3VyeV9zaGltIj4KEU1l",
                  "cmN1cnlXaXJlUmVjb3JkEgsKA3RhZxgBIAEoCRIOCgZsZW5ndGgYAiABKAUS",
                  "DAoEZGF0YRgDIAEoDGIGcHJvdG8z"));
            descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
                new pbr::FileDescriptor[] { },
                new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::MercuryShim.MercuryWireRecord), global::MercuryShim.MercuryWireRecord.Parser, new[]{ "Tag", "Length", "Data" }, null, null, null)
                }));
        }
        #endregion

    }
    #region Messages
    public sealed partial class MercuryWireRecord : pb::IMessage<MercuryWireRecord>
    {
        private static readonly pb::MessageParser<MercuryWireRecord> _parser = new pb::MessageParser<MercuryWireRecord>(() => new MercuryWireRecord());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<MercuryWireRecord> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor
        {
            get { return global::MercuryShim.MercurywirerecordReflection.Descriptor.MessageTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor
        {
            get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public MercuryWireRecord()
        {
            OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public MercuryWireRecord(MercuryWireRecord other) : this()
        {
            tag_ = other.tag_;
            length_ = other.length_;
            data_ = other.data_;
            _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public MercuryWireRecord Clone()
        {
            return new MercuryWireRecord(this);
        }

        /// <summary>Field number for the "tag" field.</summary>
        public const int TagFieldNumber = 1;
        private string tag_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Tag
        {
            get { return tag_; }
            set
            {
                tag_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }

        /// <summary>Field number for the "length" field.</summary>
        public const int LengthFieldNumber = 2;
        private int length_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int Length
        {
            get { return length_; }
            set
            {
                length_ = value;
            }
        }

        /// <summary>Field number for the "data" field.</summary>
        public const int DataFieldNumber = 3;
        private pb::ByteString data_ = pb::ByteString.Empty;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public pb::ByteString Data
        {
            get { return data_; }
            set
            {
                data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other)
        {
            return Equals(other as MercuryWireRecord);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(MercuryWireRecord other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (Tag != other.Tag) return false;
            if (Length != other.Length) return false;
            if (Data != other.Data) return false;
            return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            int hash = 1;
            if (Tag.Length != 0) hash ^= Tag.GetHashCode();
            if (Length != 0) hash ^= Length.GetHashCode();
            if (Data.Length != 0) hash ^= Data.GetHashCode();
            if (_unknownFields != null)
            {
                hash ^= _unknownFields.GetHashCode();
            }
            return hash;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override string ToString()
        {
            return pb::JsonFormatter.ToDiagnosticString(this);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output)
        {
            if (Tag.Length != 0)
            {
                output.WriteRawTag(10);
                output.WriteString(Tag);
            }
            if (Length != 0)
            {
                output.WriteRawTag(16);
                output.WriteInt32(Length);
            }
            if (Data.Length != 0)
            {
                output.WriteRawTag(26);
                output.WriteBytes(Data);
            }
            if (_unknownFields != null)
            {
                _unknownFields.WriteTo(output);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize()
        {
            int size = 0;
            if (Tag.Length != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Tag);
            }
            if (Length != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeInt32Size(Length);
            }
            if (Data.Length != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeBytesSize(Data);
            }
            if (_unknownFields != null)
            {
                size += _unknownFields.CalculateSize();
            }
            return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(MercuryWireRecord other)
        {
            if (other == null)
            {
                return;
            }
            if (other.Tag.Length != 0)
            {
                Tag = other.Tag;
            }
            if (other.Length != 0)
            {
                Length = other.Length;
            }
            if (other.Data.Length != 0)
            {
                Data = other.Data;
            }
            _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(pb::CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    default:
                        _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
                        break;
                    case 10:
                        {
                            Tag = input.ReadString();
                            break;
                        }
                    case 16:
                        {
                            Length = input.ReadInt32();
                            break;
                        }
                    case 26:
                        {
                            Data = input.ReadBytes();
                            break;
                        }
                }
            }
        }

    }

    #endregion

}

#endregion Designer generated code
